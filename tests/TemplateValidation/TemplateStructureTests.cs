using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace TemplateValidation
{
public class TemplateStructureTests
{
    private readonly string _templatesPath;

    public TemplateStructureTests()
    {
        // Navigate from test assembly location to templates folder
        var assemblyPath = AppContext.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(assemblyPath, "..", "..", "..", ".."));
        _templatesPath = Path.Combine(projectRoot, "templates");
    }

    [Theory]
    [InlineData("Auth0.BlazorServer")]
    [InlineData("Auth0.BlazorWebApp")]
    [InlineData("Auth0.BlazorWebAssembly")]
    [InlineData("Auth0.Mvc")]
    [InlineData("Auth0.WebAPI")]
    [InlineData("Auth0.Maui")]
    [InlineData("Auth0.BackendForFrontend")]
    public void Template_HasTemplateConfigFile(string templatePath)
    {
        // Arrange
        var configPath = Path.Combine(_templatesPath, templatePath, ".template.config", "template.json");

        // Act & Assert
        File.Exists(configPath).Should().BeTrue($"template.json should exist at {configPath}");
    }

    [Theory]
    [InlineData("Auth0.BlazorServer")]
    [InlineData("Auth0.BlazorWebApp/version8/Auth0BlazorWebApp")]
    [InlineData("Auth0.BlazorWebApp/version9/Auth0BlazorWebApp")]
    [InlineData("Auth0.BlazorWebApp/version10/Auth0BlazorWebApp")]
    [InlineData("Auth0.BlazorWebAssembly/version8/Client")]
    [InlineData("Auth0.BlazorWebAssembly/version8/Server")]
    [InlineData("Auth0.BlazorWebAssembly/version9/Client")]
    [InlineData("Auth0.BlazorWebAssembly/version9/Server")]
    [InlineData("Auth0.BlazorWebAssembly/version10")]
    [InlineData("Auth0.Mvc/version8")]
    [InlineData("Auth0.Mvc/version9")]
    [InlineData("Auth0.Mvc/version10")]
    [InlineData("Auth0.WebAPI/version8")]
    [InlineData("Auth0.WebAPI/version9")]
    [InlineData("Auth0.WebAPI/version10")]
    [InlineData("Auth0.MinimalWebAPI/version8")]
    [InlineData("Auth0.MinimalWebAPI/version9")]
    [InlineData("Auth0.MinimalWebAPI/version10")]
    [InlineData("Auth0.Maui/version8")]
    [InlineData("Auth0.Maui/version9")]
    [InlineData("Auth0.Maui/version10")]
    [InlineData("Auth0.BackendForFrontend/version10/Auth0BackendForFrontend.Server")]
    public void Template_HasRegistrationConfigFile(string templatePath)
    {
        // Arrange
        var configPath = Path.Combine(_templatesPath, templatePath, "registration", "config.json");

        // Act & Assert
        File.Exists(configPath).Should().BeTrue($"registration/config.json should exist at {configPath}");
    }

    [Theory]
    [InlineData("Auth0.BlazorServer")]
    [InlineData("Auth0.BlazorWebApp/version8/Auth0BlazorWebApp")]
    [InlineData("Auth0.BlazorWebApp/version9/Auth0BlazorWebApp")]
    [InlineData("Auth0.BlazorWebApp/version10/Auth0BlazorWebApp")]
    [InlineData("Auth0.Mvc/version8")]
    [InlineData("Auth0.Mvc/version9")]
    [InlineData("Auth0.Mvc/version10")]
    [InlineData("Auth0.WebAPI/version8")]
    [InlineData("Auth0.WebAPI/version9")]
    [InlineData("Auth0.WebAPI/version10")]
    [InlineData("Auth0.MinimalWebAPI/version8")]
    [InlineData("Auth0.MinimalWebAPI/version9")]
    [InlineData("Auth0.MinimalWebAPI/version10")]
    [InlineData("Auth0.Maui/version8")]
    [InlineData("Auth0.Maui/version9")]
    [InlineData("Auth0.Maui/version10")]
    [InlineData("Auth0.BackendForFrontend/version10/Auth0BackendForFrontend.Server")]
    public void Template_RegistrationConfig_IsValidJson(string templatePath)
    {
        // Arrange
        var configPath = Path.Combine(_templatesPath, templatePath, "registration", "config.json");
        var jsonContent = File.ReadAllText(configPath);

        // Act
        Action act = () => JsonSerializer.Deserialize<ConfigData>(jsonContent);

        // Assert
        act.Should().NotThrow($"config.json at {configPath} should be valid JSON");
    }

    [Theory]
    [InlineData("Auth0.BlazorServer")]
    [InlineData("Auth0.BlazorWebApp/version8/Auth0BlazorWebApp")]
    [InlineData("Auth0.BlazorWebApp/version9/Auth0BlazorWebApp")]
    [InlineData("Auth0.BlazorWebApp/version10/Auth0BlazorWebApp")]
    [InlineData("Auth0.Mvc/version8")]
    [InlineData("Auth0.Mvc/version9")]
    [InlineData("Auth0.Mvc/version10")]
    [InlineData("Auth0.WebAPI/version8")]
    [InlineData("Auth0.WebAPI/version9")]
    [InlineData("Auth0.WebAPI/version10")]
    [InlineData("Auth0.MinimalWebAPI/version8")]
    [InlineData("Auth0.MinimalWebAPI/version9")]
    [InlineData("Auth0.MinimalWebAPI/version10")]
    [InlineData("Auth0.Maui/version8")]
    [InlineData("Auth0.Maui/version9")]
    [InlineData("Auth0.Maui/version10")]
    [InlineData("Auth0.BackendForFrontend/version10/Auth0BackendForFrontend.Server")]
    public void Template_RegistrationConfig_HasRequiredProperties(string templatePath)
    {
        // Arrange
        var configPath = Path.Combine(_templatesPath, templatePath, "registration", "config.json");
        var jsonContent = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<ConfigData>(jsonContent);

        // Assert
        config.Should().NotBeNull();
        config!.AppName.Should().NotBeNullOrEmpty($"AppName is required in {configPath}");
        config.AppDescription.Should().NotBeNullOrEmpty($"AppDescription is required in {configPath}");
        config.AppType.Should().NotBeNullOrEmpty($"AppType is required in {configPath}");
        config.AppSettingsFiles.Should().NotBeEmpty($"AppSettingsFiles is required in {configPath}");
        config.RegistrationScriptFile.Should().NotBeNullOrEmpty($"RegistrationScriptFile is required in {configPath}");
    }

    [Theory]
    [InlineData("Auth0.WebAPI/version8", "api")]
    [InlineData("Auth0.WebAPI/version9", "api")]
    [InlineData("Auth0.WebAPI/version10", "api")]
    [InlineData("Auth0.MinimalWebAPI/version8", "api")]
    [InlineData("Auth0.MinimalWebAPI/version9", "api")]
    [InlineData("Auth0.MinimalWebAPI/version10", "api")]
    [InlineData("Auth0.Mvc/version8", "regular")]
    [InlineData("Auth0.Mvc/version9", "regular")]
    [InlineData("Auth0.Mvc/version10", "regular")]
    [InlineData("Auth0.BlazorServer", "regular")]
    [InlineData("Auth0.BackendForFrontend/version10/Auth0BackendForFrontend.Server", "regular")]
    [InlineData("Auth0.Maui/version8", "native")]
    [InlineData("Auth0.Maui/version9", "native")]
    [InlineData("Auth0.Maui/version10", "native")]
    public void Template_RegistrationConfig_HasCorrectAppType(string templatePath, string expectedAppType)
    {
        // Arrange
        var configPath = Path.Combine(_templatesPath, templatePath, "registration", "config.json");
        var jsonContent = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<ConfigData>(jsonContent);

        // Assert
        config.Should().NotBeNull();
        config!.AppType.Should().Be(expectedAppType, $"AppType should be '{expectedAppType}' for {templatePath}");
    }

    [Theory]
    [InlineData("Auth0.WebAPI/version8")]
    [InlineData("Auth0.WebAPI/version9")]
    [InlineData("Auth0.WebAPI/version10")]
    [InlineData("Auth0.MinimalWebAPI/version8")]
    [InlineData("Auth0.MinimalWebAPI/version9")]
    [InlineData("Auth0.MinimalWebAPI/version10")]
    public void ApiTemplate_HasEmptyCallbacksAndLogoutUrls(string templatePath)
    {
        // Arrange
        var configPath = Path.Combine(_templatesPath, templatePath, "registration", "config.json");
        var jsonContent = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<ConfigData>(jsonContent);

        // Assert
        config.Should().NotBeNull();
        config!.Callbacks.Should().BeEmpty($"API templates should have empty Callbacks: {templatePath}");
        config.LogoutUrls.Should().BeEmpty($"API templates should have empty LogoutUrls: {templatePath}");
    }

    [Theory]
    [InlineData("Auth0.Mvc/version8")]
    [InlineData("Auth0.Mvc/version9")]
    [InlineData("Auth0.Mvc/version10")]
    [InlineData("Auth0.BlazorServer")]
    [InlineData("Auth0.BlazorWebApp/version8/Auth0BlazorWebApp")]
    [InlineData("Auth0.BlazorWebApp/version9/Auth0BlazorWebApp")]
    [InlineData("Auth0.BlazorWebApp/version10/Auth0BlazorWebApp")]
    [InlineData("Auth0.BackendForFrontend/version10/Auth0BackendForFrontend.Server")]
    public void WebTemplate_HasCallbacksAndLogoutUrls(string templatePath)
    {
        // Arrange
        var configPath = Path.Combine(_templatesPath, templatePath, "registration", "config.json");
        var jsonContent = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<ConfigData>(jsonContent);

        // Assert
        config.Should().NotBeNull();
        config!.Callbacks.Should().NotBeEmpty($"Web templates should have Callbacks: {templatePath}");
        config.LogoutUrls.Should().NotBeEmpty($"Web templates should have LogoutUrls: {templatePath}");
    }

    [Theory]
    [InlineData("Auth0.Maui/version8")]
    [InlineData("Auth0.Maui/version9")]
    [InlineData("Auth0.Maui/version10")]
    public void MauiTemplate_HasCallbacksAndLogoutUrls(string templatePath)
    {
        // Arrange
        var configPath = Path.Combine(_templatesPath, templatePath, "registration", "config.json");
        var jsonContent = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<ConfigData>(jsonContent);

        // Assert
        config.Should().NotBeNull();
        config!.Callbacks.Should().NotBeEmpty($"MAUI templates should have Callbacks: {templatePath}");
        config.LogoutUrls.Should().NotBeEmpty($"MAUI templates should have LogoutUrls: {templatePath}");
    }

    [Fact]
    public void AllTemplates_HaveCliWrapperBinaries()
    {
        // Arrange
        var templateDirs = GetAllTemplatePaths();

        foreach (var templateDir in templateDirs)
        {
            var registrationPath = Path.Combine(templateDir, "registration");
            if (!Directory.Exists(registrationPath)) continue;

            // Act & Assert
            var dllPath = Path.Combine(registrationPath, "cli-wrapper.dll");
            File.Exists(dllPath).Should().BeTrue(
                $"cli-wrapper.dll should exist in {registrationPath}. " +
                "Run 'dotnet pack' to copy CLI wrapper binaries to all registration folders."
            );
        }
    }

    private string[] GetAllTemplatePaths()
    {
        return new[]
        {
            Path.Combine(_templatesPath, "Auth0.BlazorServer"),
            Path.Combine(_templatesPath, "Auth0.BlazorWebApp", "version8", "Auth0BlazorWebApp"),
            Path.Combine(_templatesPath, "Auth0.BlazorWebApp", "version9", "Auth0BlazorWebApp"),
            Path.Combine(_templatesPath, "Auth0.BlazorWebApp", "version10", "Auth0BlazorWebApp"),
            Path.Combine(_templatesPath, "Auth0.BlazorWebAssembly", "version8", "Client"),
            Path.Combine(_templatesPath, "Auth0.BlazorWebAssembly", "version8", "Server"),
            Path.Combine(_templatesPath, "Auth0.BlazorWebAssembly", "version9", "Client"),
            Path.Combine(_templatesPath, "Auth0.BlazorWebAssembly", "version9", "Server"),
            Path.Combine(_templatesPath, "Auth0.BlazorWebAssembly", "version10"),
            Path.Combine(_templatesPath, "Auth0.Mvc", "version8"),
            Path.Combine(_templatesPath, "Auth0.Mvc", "version9"),
            Path.Combine(_templatesPath, "Auth0.Mvc", "version10"),
            Path.Combine(_templatesPath, "Auth0.WebAPI", "version8"),
            Path.Combine(_templatesPath, "Auth0.WebAPI", "version9"),
            Path.Combine(_templatesPath, "Auth0.WebAPI", "version10"),
            Path.Combine(_templatesPath, "Auth0.MinimalWebAPI", "version8"),
            Path.Combine(_templatesPath, "Auth0.MinimalWebAPI", "version9"),
            Path.Combine(_templatesPath, "Auth0.MinimalWebAPI", "version10"),
            Path.Combine(_templatesPath, "Auth0.Maui", "version8"),
            Path.Combine(_templatesPath, "Auth0.Maui", "version9"),
            Path.Combine(_templatesPath, "Auth0.Maui", "version10"),
            Path.Combine(_templatesPath, "Auth0.BackendForFrontend", "version10", "Auth0BackendForFrontend.Server")
        };
    }
}
}
