# Auth0 Templates Test Suite

This directory contains automated tests for the Auth0 Templates for .NET project. The test suite validates the CLI wrapper business logic, template structure, and generated project correctness.

## Test Organization

```
tests/
├── Unit/                          # Fast unit tests for pure logic
│   ├── CliWrapperTests.cs        # Core CLI wrapper logic
│   ├── ConfigDataTests.cs        # Configuration model tests
│   ├── RegistrationDataTests.cs  # Registration data serialization
│   ├── TenantDataTests.cs        # Tenant data parsing
│   └── DisplayerTests.cs         # Output formatting tests
├── Integration/                   # Integration tests with mocked dependencies
│   ├── RegistrationIntegrationTests.cs    # Full registration workflows
│   └── ConfigUpdateIntegrationTests.cs    # Config file update logic
├── TemplateValidation/           # Template structure and instantiation tests
│   ├── TemplateStructureTests.cs          # Validates template files exist
│   └── TemplateInstantiationTests.cs      # End-to-end template generation
└── TestHelpers/                   # Shared test utilities
    ├── MockResponseLoader.cs     # Loads mock Auth0 CLI responses
    ├── TempTestEnvironment.cs    # Creates temp test directories
    └── MockResponses/            # JSON files with mock CLI output
```

## Test Categories

### Unit Tests
Fast tests that verify individual methods and pure logic without external dependencies:
- String manipulation (CreateAudience)
- Version comparison logic
- JSON serialization/deserialization
- Output formatting behavior

**Run unit tests only:**
```bash
dotnet test --filter "FullyQualifiedName~Unit"
```

### Integration Tests
Tests that verify complete workflows with mocked Auth0 CLI:
- Full registration process (app and API creation)
- Configuration file updates with various CLI versions
- Domain extraction strategies (tenants list vs signing keys)
- Error handling and fallback behavior

**Run integration tests only:**
```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

### Template Validation Tests
Tests that scan and validate all template variants:
- Verify template.json files exist and are valid
- Check registration/config.json consistency
- Validate AppType, Callbacks, and LogoutUrls correctness
- Ensure CLI wrapper binaries are present

**Run template validation tests:**
```bash
dotnet test --filter "FullyQualifiedName~TemplateValidation.TemplateStructureTests"
```

### Template Instantiation Tests
End-to-end tests that actually generate projects from templates:
- Instantiate each template variant
- Verify generated files and configuration
- Ensure generated projects compile successfully

**Note:** These tests are skipped by default because they require the template package to be built first.

**To run instantiation tests:**
```bash
# First, build the template package
cd ..
dotnet pack

# Then run the tests
cd tests
dotnet test --filter "FullyQualifiedName~TemplateInstantiationTests"
```

## Running Tests

### Run All Tests
```bash
cd tests
dotnet test
```

### Run Tests with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

Coverage reports will be generated in the `TestResults/` directory.

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~CliWrapperTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "FullyQualifiedName~CliWrapperTests.CreateAudience_TransformsAppNameToValidUrl"
```

### Verbose Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Test Frameworks and Libraries

- **.NET 10.0**: Target framework
- **xUnit v3 (3.2.2)**: Test framework
- **FluentAssertions 8.8+**: Readable assertions
- **Moq 4.20+**: Mocking framework for Auth0 CLI process execution
- **coverlet.collector 8.0+**: Code coverage collection

## Mock Strategy

The CLI wrapper communicates with the Auth0 CLI via process execution. To test this without requiring an actual Auth0 tenant or CLI installation:

1. **IProcessExecutor Interface**: CliWrapper accepts an `IProcessExecutor` for dependency injection
2. **ProcessExecutor**: Production implementation that executes real processes
3. **Mock<IProcessExecutor>**: Test implementation using Moq that returns predefined responses

This allows tests to:
- Run without Auth0 CLI installed
- Avoid network calls and rate limiting
- Simulate error conditions
- Run quickly and reliably

## Mock Response Files

The `TestHelpers/MockResponses/` directory contains JSON files that simulate Auth0 CLI output:

- `apps-create-success.json`: Successful app registration response
- `apis-create-success.json`: Successful API registration response
- `tenant-list-v1.17.json`: Tenant list from modern CLI (v1.17.0+)
- `apis-list-v1.16.json`: APIs list from older CLI (< v1.17.0)
- `cli-version-*.txt`: CLI version check responses

Use `MockResponseLoader` to load these files in tests.

## Adding New Tests

### 1. Unit Test for New Method

```csharp
[Fact]
public void NewMethod_DoesWhat_UnderCondition()
{
    // Arrange
    var mockExecutor = new Mock<IProcessExecutor>();
    var wrapper = new CliWrapper(mockExecutor.Object);

    // Act
    var result = wrapper.NewMethod("input");

    // Assert
    result.Should().Be("expected");
}
```

### 2. Integration Test with Mocked CLI

```csharp
[Fact]
public async Task NewWorkflow_Works_WithMockedCli()
{
    // Arrange
    var mockExecutor = new Mock<IProcessExecutor>();
    mockExecutor
        .Setup(e => e.RunCommandAsync("auth0", It.IsAny<string>()))
        .ReturnsAsync("{ \"result\": \"success\" }");

    var wrapper = new CliWrapper(mockExecutor.Object);

    // Act
    var result = await wrapper.NewWorkflow();

    // Assert
    result.Should().NotBeNull();
    mockExecutor.Verify(
        e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("expected arg"))),
        Times.Once
    );
}
```

### 3. Template Validation Test

```csharp
[Theory]
[InlineData("Auth0.NewTemplate/version8")]
public void NewTemplate_HasRequiredFiles(string templatePath)
{
    // Arrange
    var fullPath = Path.Combine(_templatesPath, templatePath);

    // Act & Assert
    File.Exists(Path.Combine(fullPath, ".template.config", "template.json"))
        .Should().BeTrue();
    File.Exists(Path.Combine(fullPath, "registration", "config.json"))
        .Should().BeTrue();
}
```

## Best Practices

1. **Use descriptive test names**: `Method_Does_WhenCondition`
2. **Follow AAA pattern**: Arrange, Act, Assert
3. **One assertion per test** (when practical)
4. **Use FluentAssertions** for readable assertions
5. **Clean up resources**: Use `IDisposable` for temp files/directories
6. **Mock external dependencies**: Never call real Auth0 CLI or network services
7. **Make tests independent**: Each test should run in isolation
8. **Use Theory for data-driven tests**: Test multiple inputs with `[Theory]` and `[InlineData]`

## Continuous Integration

While CI/CD is not currently configured, these tests are designed to run in automated pipelines:

- **Fast execution**: Most tests complete in < 1 second
- **No external dependencies**: All Auth0 CLI interactions are mocked
- **Deterministic**: Tests produce consistent results
- **Cross-platform**: Tests run on Windows, macOS, and Linux

To add GitHub Actions:
```yaml
name: Tests
on: [push, pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --verbosity normal
```

## Troubleshooting

### "Template package not found" error
Template instantiation tests require the NuGet package to be built first:
```bash
cd ..
dotnet pack
```

### "config.json not found" in tests
The CliWrapper constructor reads `config.json` from `AppContext.BaseDirectory`. Tests create this file programmatically. If you see this error, check that:
1. The test creates a config.json file
2. File permissions allow writing to the test directory

### Tests fail with "Access denied" on cleanup
Windows may lock files during test execution. The `Dispose()` methods catch and ignore cleanup errors. If this becomes problematic, ensure tests are properly disposing resources.

### Mock verification fails
Check that:
1. The expected command/arguments match exactly
2. You're using `It.Is<string>()` with a predicate for partial matches
3. The mock setup is called before the method under test

## Coverage Goals

Target code coverage metrics:
- **CLI Wrapper Core Logic**: > 80%
- **Data Models**: > 90%
- **Overall Project**: > 70%

View coverage report:
```bash
dotnet test --collect:"XPlat Code Coverage"
# Install reportgenerator if not already installed
dotnet tool install -g dotnet-reportgenerator-globaltool
# Generate HTML report
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport"
# Open TestResults/CoverageReport/index.html in browser
```

## Contributing

When adding new features to the CLI wrapper or templates:

1. Write tests first (TDD) or alongside implementation
2. Ensure all existing tests still pass
3. Add integration tests for new workflows
4. Update template validation tests if adding new templates
5. Document any new mock responses needed

Tests are a key part of the contribution process. See [CONTRIBUTING.md](../CONTRIBUTING.md) for more details.
