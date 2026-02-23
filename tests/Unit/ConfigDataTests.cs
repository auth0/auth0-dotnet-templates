using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Unit
{
public class ConfigDataTests
{
    [Fact]
    public void ConfigData_Deserializes_FromValidJson()
    {
        // Arrange
        var json = @"{
            ""AppName"": ""TestApp"",
            ""AppDescription"": ""Test application description"",
            ""AppType"": ""regular"",
            ""Callbacks"": ""https://localhost:5001/callback"",
            ""LogoutUrls"": ""https://localhost:5001/"",
            ""AppSettingsFiles"": [""./appsettings.json"", ""./appsettings.Development.json""],
            ""RegistrationScriptFile"": ""../register-with-auth0.cmd"",
            ""Verbose"": true
        }";

        // Act
        var config = JsonSerializer.Deserialize<ConfigData>(json);

        // Assert
        config.Should().NotBeNull();
        config!.AppName.Should().Be("TestApp");
        config.AppDescription.Should().Be("Test application description");
        config.AppType.Should().Be("regular");
        config.Callbacks.Should().Be("https://localhost:5001/callback");
        config.LogoutUrls.Should().Be("https://localhost:5001/");
        config.AppSettingsFiles.Should().HaveCount(2);
        config.AppSettingsFiles[0].Should().Be("./appsettings.json");
        config.RegistrationScriptFile.Should().Be("../register-with-auth0.cmd");
        config.Verbose.Should().BeTrue();
    }

    [Fact]
    public void ConfigData_Serializes_ToValidJson()
    {
        // Arrange
        var config = new ConfigData
        {
            AppName = "MyApp",
            AppDescription = "My Description",
            AppType = "spa",
            Callbacks = "https://localhost:3000/callback",
            LogoutUrls = "https://localhost:3000/",
            AppSettingsFiles = new[] { "./appsettings.json" },
            RegistrationScriptFile = "../register.cmd",
            Verbose = false
        };

        // Act
        var json = JsonSerializer.Serialize(config);
        var deserialized = JsonSerializer.Deserialize<ConfigData>(json);

        // Assert
        deserialized.Should().BeEquivalentTo(config);
    }

    [Fact]
    public void ConfigData_HasDefaultValues()
    {
        // Act
        var config = new ConfigData();

        // Assert
        config.AppName.Should().BeEmpty();
        config.AppDescription.Should().BeEmpty();
        config.AppType.Should().BeEmpty();
        config.Callbacks.Should().BeEmpty();
        config.LogoutUrls.Should().BeEmpty();
        config.AppSettingsFiles.Should().BeEmpty();
        config.RegistrationScriptFile.Should().BeEmpty();
        config.Verbose.Should().BeFalse();
    }
}
}
