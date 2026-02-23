# Contribution

Please read [Auth0's contribution guidelines](https://github.com/auth0/open-source-template/blob/master/GENERAL-CONTRIBUTING.md).

## Environment setup

- Make sure you have the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) installed.
- Follow the local development steps below to get started.

## Local development

- `dotnet pack`: create a local NuGet package in the `./bin/Debug/` folder.
- `dotnet new -i ./output/Auth0.Templates.x.y.z.nupkg`: install the local NuGet package version x.y.z
- `npm run pack-install`: if you have Node.js installed, it installs or updates the local NuGet package

## Testing

### Automated Tests

The project includes a comprehensive automated test suite in the `tests/` directory covering:

- **Unit tests**: CLI wrapper business logic, data models, and output formatting
- **Integration tests**: Full registration workflows and configuration updates with mocked Auth0 CLI
- **Template validation tests**: Structure and consistency checks for all template variants
- **Template instantiation tests**: End-to-end verification that templates generate working projects

#### Running Tests

Run all fast tests (unit + integration):
```bash
cd tests
dotnet test
```

Run tests with code coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

Run template validation tests:
```bash
dotnet test --filter "FullyQualifiedName~TemplateValidation.TemplateStructureTests"
```

Run template instantiation tests (requires `dotnet pack` to be run first):
```bash
dotnet pack
cd tests
dotnet test --filter "FullyQualifiedName~TemplateInstantiationTests"
```

For detailed information about the test suite, see [tests/README.md](tests/README.md).

### Manual Tests

Manual tests should still be performed on:
- .NET CLI
- Visual Studio 2022 for Windows
- Visual Studio 2022 for Mac
- JetBrains Rider

Test each template variant across different .NET versions (8, 9, 10) to ensure compatibility.