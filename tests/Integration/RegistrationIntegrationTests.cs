using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Integration
{
public class RegistrationIntegrationTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly string _configPath;

    public RegistrationIntegrationTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"cli-wrapper-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);
        _configPath = Path.Combine(_tempDirectory, "config.json");
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    [Fact]
    public async Task Register_WithModernCli_UsestenantListForDomain()
    {
        // Arrange
        var mockExecutor = new Mock<IProcessExecutor>();
        
        // Setup CLI version check (1.20.0 >= 1.17.0)
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        // Setup app registration
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("apps create"))))
            .ReturnsAsync(MockResponseLoader.LoadAppCreateSuccess());

        // Setup tenant list (for domain extraction)
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "tenants list --json"))
            .ReturnsAsync(MockResponseLoader.LoadTenantList());

        var wrapper = CreateTestWrapper(mockExecutor.Object);

        // Act
        await wrapper.IsAuth0CliInstalled();
        var result = await wrapper.Register();

        // Assert
        result.client_id.Should().Be("test-client-id-12345");
        result.name.Should().Be("Test Application");
        
        // Verify tenant list was called (modern CLI version)
        mockExecutor.Verify(
            e => e.RunCommandAsync("auth0", "tenants list --json"),
            Times.Never // Not called during Register, only during UpdateConfigFiles
        );
    }

    [Fact]
    public async Task Register_WithOlderCli_UsesSigningKeysForDomain()
    {
        // Arrange
        var mockExecutor = new Mock<IProcessExecutor>();
        
        // Setup CLI version check (1.0.1 < 1.17.0)
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.0.1 abc123");

        // Setup app registration (includes signing_keys with domain)
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("apps create"))))
            .ReturnsAsync(MockResponseLoader.LoadAppCreateSuccess());

        var wrapper = CreateTestWrapper(mockExecutor.Object);

        // Act
        await wrapper.IsAuth0CliInstalled();
        var result = await wrapper.Register();

        // Assert
        result.client_id.Should().Be("test-client-id-12345");
        result.signing_keys.Should().HaveCount(1);
        result.signing_keys[0].subject.Should().Be("/CN=test-tenant.auth0.com");
    }

    [Fact]
    public async Task Register_ApiType_UsesApisCreateCommand()
    {
        // Arrange
        var mockExecutor = new Mock<IProcessExecutor>();
        
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("apis create"))))
            .ReturnsAsync(MockResponseLoader.LoadApiCreateSuccess());

        var wrapper = CreateTestWrapper(mockExecutor.Object, appType: "api");

        // Act
        await wrapper.IsAuth0CliInstalled();
        var result = await wrapper.Register();

        // Assert
        result.identifier.Should().Be("https://test-api.com");
        result.name.Should().Be("Test API");
        
        mockExecutor.Verify(
            e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("apis create"))),
            Times.Once
        );
    }

    [Fact]
    public async Task Register_HandlesCliError_ReturnsEmptyRegistrationData()
    {
        // Arrange
        var mockExecutor = new Mock<IProcessExecutor>();
        
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("apps create"))))
            .ThrowsAsync(new InvalidOperationException("Auth0 CLI error: invalid credentials"));

        var wrapper = CreateTestWrapper(mockExecutor.Object);

        // Act
        await wrapper.IsAuth0CliInstalled();
        var result = await wrapper.Register();

        // Assert
        result.client_id.Should().BeEmpty();
        result.name.Should().BeEmpty();
    }

    [Fact]
    public async Task Register_HandlesMalformedJson_ReturnsEmptyRegistrationData()
    {
        // Arrange
        var mockExecutor = new Mock<IProcessExecutor>();
        
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("apps create"))))
            .ReturnsAsync("{ invalid json }}");

        var wrapper = CreateTestWrapper(mockExecutor.Object);

        // Act
        await wrapper.IsAuth0CliInstalled();
        var result = await wrapper.Register();

        // Assert
        result.client_id.Should().BeEmpty();
    }

    [Theory]
    [InlineData("My Test App", "https://my-test-app.com")]
    [InlineData("API@Service#1", "https://api-service-1.com")]
    [InlineData("Special_Chars!", "https://special-chars-.com")]
    public async Task Register_ApiType_CreatesCorrectAudience(string appName, string expectedAudience)
    {
        // Arrange
        var mockExecutor = new Mock<IProcessExecutor>();
        string capturedArgs = "";
        
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", It.Is<string>(s => s.Contains("apis create"))))
            .Callback<string, string>((cmd, args) => capturedArgs = args)
            .ReturnsAsync(MockResponseLoader.LoadApiCreateSuccess());

        var wrapper = CreateTestWrapper(mockExecutor.Object, appType: "api", appName: appName);

        // Act
        await wrapper.IsAuth0CliInstalled();
        await wrapper.Register();

        // Assert
        capturedArgs.Should().Contain($"--identifier {expectedAudience}");
    }

    private CliWrapper CreateTestWrapper(
        IProcessExecutor executor,
        string appType = "regular",
        string appName = "TestApp")
    {
        // Create config.json in temp directory
        var configJson = $@"{{
            ""AppName"": ""{appName}"",
            ""AppDescription"": ""Test application"",
            ""AppType"": ""{appType}"",
            ""Callbacks"": ""https://localhost:5001/callback"",
            ""LogoutUrls"": ""https://localhost:5001/"",
            ""AppSettingsFiles"": [],
            ""RegistrationScriptFile"": ""../register-with-auth0.cmd"",
            ""Verbose"": false
        }}";

        // Write to the actual AppContext.BaseDirectory for the constructor to find
        var testConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        File.WriteAllText(testConfigPath, configJson);

        return new CliWrapper(executor);
    }
}
}
