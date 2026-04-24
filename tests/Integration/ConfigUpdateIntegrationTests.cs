using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Integration
{
public class ConfigUpdateIntegrationTests : IDisposable
{
    private readonly string _tempDirectory;

    public ConfigUpdateIntegrationTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"cli-wrapper-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);
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
    public async Task UpdateConfigFiles_ReplacesPlaceholders_WithActualValues()
    {
        // Arrange
        var appSettingsPath = Path.Combine(_tempDirectory, "appsettings.json");
        var originalContent = @"{
  ""Auth0"": {
    ""Domain"": ""yourdomain.auth0.com"",
    ""ClientId"": ""your-client-id""
  }
}";
        File.WriteAllText(appSettingsPath, originalContent);

        var mockExecutor = new Mock<IProcessExecutor>();
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "tenants list --json"))
            .ReturnsAsync(@"[{""name"": ""test-tenant.auth0.com"", ""active"": true}]");

        var wrapper = CreateTestWrapper(mockExecutor.Object, new[] { appSettingsPath });
        
        var registrationData = new RegistrationData
        {
            client_id = "actual-client-id-123",
            signing_keys = new[] { new SigningKeys("/CN=test-tenant.auth0.com") }
        };

        await wrapper.IsAuth0CliInstalled();

        // Act
        await wrapper.UpdateConfigFiles(registrationData);

        // Assert
        var updatedContent = await File.ReadAllTextAsync(appSettingsPath);
        updatedContent.Should().NotContain("yourdomain.auth0.com");
        updatedContent.Should().Contain("test-tenant.auth0.com");
        updatedContent.Should().NotContain("your-client-id");
        updatedContent.Should().Contain("actual-client-id-123");
    }

    [Fact]
    public async Task UpdateConfigFiles_ReplacesApiIdentifier_ForApiType()
    {
        // Arrange
        var appSettingsPath = Path.Combine(_tempDirectory, "appsettings.json");
        var originalContent = @"{
  ""Auth0"": {
    ""Domain"": ""yourdomain.auth0.com"",
    ""Audience"": ""https://your-api-id.com""
  }
}";
        File.WriteAllText(appSettingsPath, originalContent);

        var mockExecutor = new Mock<IProcessExecutor>();
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "tenants list --json"))
            .ReturnsAsync(@"[{""name"": ""test-tenant.auth0.com"", ""active"": true}]");

        var wrapper = CreateTestWrapper(mockExecutor.Object, new[] { appSettingsPath });
        
        var registrationData = new RegistrationData
        {
            identifier = "https://my-actual-api.com",
            signing_keys = Array.Empty<SigningKeys>()
        };

        await wrapper.IsAuth0CliInstalled();

        // Act
        await wrapper.UpdateConfigFiles(registrationData);

        // Assert
        var updatedContent = await File.ReadAllTextAsync(appSettingsPath);
        updatedContent.Should().NotContain("https://your-api-id.com");
        updatedContent.Should().Contain("https://my-actual-api.com");
    }

    [Fact]
    public async Task UpdateConfigFiles_UpdatesMultipleFiles()
    {
        // Arrange
        var appSettingsPath = Path.Combine(_tempDirectory, "appsettings.json");
        
        var originalContent = @"{""Auth0"": {""ClientId"": ""your-client-id""}}";
        File.WriteAllText(appSettingsPath, originalContent);

        var mockExecutor = new Mock<IProcessExecutor>();
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "tenants list --json"))
            .ReturnsAsync(@"[{""name"": ""test-tenant.auth0.com"", ""active"": true}]");

        var wrapper = CreateTestWrapper(
            mockExecutor.Object,
            new[] { appSettingsPath }
        );
        
        var registrationData = new RegistrationData
        {
            client_id = "updated-client-id",
            signing_keys = Array.Empty<SigningKeys>()
        };

        await wrapper.IsAuth0CliInstalled();

        // Act
        await wrapper.UpdateConfigFiles(registrationData);

        // Assert
        var updatedContent = await File.ReadAllTextAsync(appSettingsPath);
        
        updatedContent.Should().Contain("updated-client-id");
    }

    [Fact]
    public async Task UpdateConfigFiles_ReplacesClientSecret_WhenClientSecretIsProvided()
    {
        // Arrange
        var appSettingsPath = Path.Combine(_tempDirectory, "appsettings.json");
        var originalContent = @"{
  ""Auth0"": {
    ""Domain"": ""yourdomain.auth0.com"",
    ""ClientId"": ""your-client-id"",
    ""ClientSecret"": ""your-client-secret""
  }
}";
        File.WriteAllText(appSettingsPath, originalContent);

        var mockExecutor = new Mock<IProcessExecutor>();
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "tenants list --json"))
            .ReturnsAsync(@"[{""name"": ""test-tenant.auth0.com"", ""active"": true}]");

        var wrapper = CreateTestWrapper(mockExecutor.Object, new[] { appSettingsPath });

        var registrationData = new RegistrationData
        {
            client_id = "actual-client-id-123",
            client_secret = "actual-client-secret-xyz",
            signing_keys = Array.Empty<SigningKeys>()
        };

        await wrapper.IsAuth0CliInstalled();

        // Act
        await wrapper.UpdateConfigFiles(registrationData);

        // Assert
        var updatedContent = await File.ReadAllTextAsync(appSettingsPath);
        updatedContent.Should().NotContain("your-client-secret");
        updatedContent.Should().Contain("actual-client-secret-xyz");
    }

    [Fact]
    public async Task UpdateConfigFiles_DoesNotReplaceClientSecretPlaceholder_WhenClientSecretIsEmpty()
    {
        // Arrange
        var appSettingsPath = Path.Combine(_tempDirectory, "appsettings.json");
        var originalContent = @"{
  ""Auth0"": {
    ""ClientSecret"": ""your-client-secret""
  }
}";
        File.WriteAllText(appSettingsPath, originalContent);

        var mockExecutor = new Mock<IProcessExecutor>();
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "tenants list --json"))
            .ReturnsAsync(@"[{""name"": ""test-tenant.auth0.com"", ""active"": true}]");

        var wrapper = CreateTestWrapper(mockExecutor.Object, new[] { appSettingsPath });

        var registrationData = new RegistrationData
        {
            client_secret = "",
            signing_keys = Array.Empty<SigningKeys>()
        };

        await wrapper.IsAuth0CliInstalled();

        // Act
        await wrapper.UpdateConfigFiles(registrationData);

        // Assert
        var updatedContent = await File.ReadAllTextAsync(appSettingsPath);
        updatedContent.Should().Contain("your-client-secret");
    }

    [Fact]
    public async Task UpdateConfigFiles_WithModernCli_UsesTenantListForDomain()
    {
        // Arrange
        var appSettingsPath = Path.Combine(_tempDirectory, "appsettings.json");
        var originalContent = @"{""Auth0"": {""Domain"": ""yourdomain.auth0.com""}}";
        File.WriteAllText(appSettingsPath, originalContent);

        var mockExecutor = new Mock<IProcessExecutor>();
        
        // Modern CLI version (>= 1.17.0)
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.20.0 abc123");

        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "tenants list --json"))
            .ReturnsAsync(@"[
                {""name"": ""inactive-tenant.auth0.com"", ""active"": false},
                {""name"": ""active-tenant.auth0.com"", ""active"": true}
            ]");

        var wrapper = CreateTestWrapper(mockExecutor.Object, new[] { appSettingsPath });
        
        var registrationData = new RegistrationData
        {
            signing_keys = Array.Empty<SigningKeys>()
        };

        await wrapper.IsAuth0CliInstalled();

        // Act
        await wrapper.UpdateConfigFiles(registrationData);

        // Assert
        var updatedContent = await File.ReadAllTextAsync(appSettingsPath);
        updatedContent.Should().Contain("active-tenant.auth0.com");
        updatedContent.Should().NotContain("inactive-tenant.auth0.com");
        
        mockExecutor.Verify(
            e => e.RunCommandAsync("auth0", "tenants list --json"),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateConfigFiles_WithOlderCli_UsesSigningKeysForDomain()
    {
        // Arrange
        var appSettingsPath = Path.Combine(_tempDirectory, "appsettings.json");
        var originalContent = @"{""Auth0"": {""Domain"": ""yourdomain.auth0.com""}}";
        File.WriteAllText(appSettingsPath, originalContent);

        var mockExecutor = new Mock<IProcessExecutor>();
        
        // Older CLI version (< 1.17.0)
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.16.0 abc123");

        var wrapper = CreateTestWrapper(mockExecutor.Object, new[] { appSettingsPath });
        
        var registrationData = new RegistrationData
        {
            signing_keys = new[] { new SigningKeys("/CN=legacy-tenant.auth0.com") }
        };

        await wrapper.IsAuth0CliInstalled();

        // Act
        await wrapper.UpdateConfigFiles(registrationData);

        // Assert
        var updatedContent = await File.ReadAllTextAsync(appSettingsPath);
        updatedContent.Should().Contain("legacy-tenant.auth0.com");
        
        // Should NOT call tenant list with older CLI
        mockExecutor.Verify(
            e => e.RunCommandAsync("auth0", "tenants list --json"),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateConfigFiles_WithOlderCli_FallsBackToApisListWhenNoSigningKeys()
    {
        // Arrange
        var appSettingsPath = Path.Combine(_tempDirectory, "appsettings.json");
        var originalContent = @"{""Auth0"": {""Domain"": ""yourdomain.auth0.com""}}";
        File.WriteAllText(appSettingsPath, originalContent);

        var mockExecutor = new Mock<IProcessExecutor>();
        
        // Older CLI version
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "--version"))
            .ReturnsAsync("auth0 version 1.16.0 abc123");

        // Mock apis list fallback
        mockExecutor
            .Setup(e => e.RunCommandAsync("auth0", "apis list -n 1 --json"))
            .ReturnsAsync(@"[{""identifier"": ""https://fallback-tenant.auth0.com/api/v2/""}]");

        var wrapper = CreateTestWrapper(mockExecutor.Object, new[] { appSettingsPath });
        
        var registrationData = new RegistrationData
        {
            signing_keys = Array.Empty<SigningKeys>() // No signing keys to extract domain from
        };

        await wrapper.IsAuth0CliInstalled();

        // Act
        await wrapper.UpdateConfigFiles(registrationData);

        // Assert
        var updatedContent = await File.ReadAllTextAsync(appSettingsPath);
        updatedContent.Should().Contain("fallback-tenant.auth0.com");
        
        mockExecutor.Verify(
            e => e.RunCommandAsync("auth0", "apis list -n 1 --json"),
            Times.Once
        );
    }

    private CliWrapper CreateTestWrapper(IProcessExecutor executor, string[] appSettingsFiles)
    {
        // Create config.json in AppContext.BaseDirectory
        var configJson = $@"{{
            ""AppName"": ""TestApp"",
            ""AppDescription"": ""Test application"",
            ""AppType"": ""regular"",
            ""Callbacks"": ""https://localhost:5001/callback"",
            ""LogoutUrls"": ""https://localhost:5001/"",
            ""AppSettingsFiles"": [{string.Join(",", Array.ConvertAll(appSettingsFiles, f => $@"""{f}"""))}],
            ""RegistrationScriptFile"": ""../register-with-auth0.cmd"",
            ""Verbose"": false
        }}";

        var testConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        File.WriteAllText(testConfigPath, configJson);

        return new CliWrapper(executor);
    }
}
}
