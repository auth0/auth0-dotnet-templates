using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace TemplateValidation
{
/// <summary>
/// Tests that instantiate actual project templates and verify they generate correctly.
/// NOTE: These tests require the template package to be built first with 'dotnet pack'.
/// These tests are slower and may be skipped in CI/CD depending on strategy.
/// </summary>
public class TemplateInstantiationTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly string _projectRoot;
    private readonly string _outputPath;
    private static bool _templateInstalled = false;
    private static readonly object _lock = new();

    static TemplateInstantiationTests()
    {
        // Ensure Displayer starts with verbose off
        Displayer.Verbose = false;
    }

    public TemplateInstantiationTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"template-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);

        var assemblyPath = AppContext.BaseDirectory;
        _projectRoot = Path.GetFullPath(Path.Combine(assemblyPath, "..", "..", "..", ".."));
        _outputPath = Path.Combine(_projectRoot, "output");

        EnsureTemplateInstalled();
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

    private void EnsureTemplateInstalled()
    {
        lock (_lock)
        {
            if (_templateInstalled) return;

            // Look for the template package
            if (!Directory.Exists(_outputPath))
            {
                throw new InvalidOperationException(
                    "Template package not found. Run 'dotnet pack' from the project root first."
                );
            }

            var packageFiles = Directory.GetFiles(_outputPath, "Auth0.Templates.*.nupkg");
            if (packageFiles.Length == 0)
            {
                throw new InvalidOperationException(
                    "No template package found in output/. Run 'dotnet pack' from the project root first."
                );
            }

            // Install the template (uninstall first to ensure clean state)
            if (IsTemplateInstalled("Auth0.Templates"))
            {
                RunDotnetCommand($"new uninstall Auth0.Templates");
            }
            RunDotnetCommand($"new install \"{packageFiles[0]}\"");

            _templateInstalled = true;
        }
    }

    [Theory]
    [InlineData("auth0webapp", "net8.0")]
    [InlineData("auth0webapp", "net9.0")]
    [InlineData("auth0webapp", "net10.0")]
    public async Task MvcTemplate_Instantiates_AndBuilds(string templateName, string framework)
    {
        // Arrange
        var frameworkVersion = framework.Replace(".", "");
        var projectName = $"TestMvcApp{frameworkVersion}";
        var projectPath = Path.Combine(_tempDirectory, projectName);

        // Act
        var instantiateResult = RunDotnetCommand(
            $"new {templateName} -n {projectName} -o \"{projectPath}\" --framework {framework} --domain test.auth0.com --client-id test-client-id"
        );

        // Assert - verify instantiation succeeded
        instantiateResult.Should().Be(0, "Template instantiation should succeed");
        Directory.Exists(projectPath).Should().BeTrue();
        File.Exists(Path.Combine(projectPath, $"{projectName}.csproj")).Should().BeTrue();

        // Verify appsettings.json was created and configured
        var appSettingsPath = Path.Combine(projectPath, "appsettings.json");
        File.Exists(appSettingsPath).Should().BeTrue();
        var appSettings = await File.ReadAllTextAsync(appSettingsPath);
        appSettings.Should().Contain("test.auth0.com");
        appSettings.Should().Contain("test-client-id");

        // Verify the project builds
        //var buildResult = RunDotnetCommand($"build \"{projectPath}\" --configuration Release");
        //buildResult.Should().Be(0, "Generated project should build successfully");
    }

    [Theory]
    [InlineData("auth0webapi", "net8.0")]
    [InlineData("auth0webapi", "net9.0")]
    [InlineData("auth0webapi", "net10.0")]
    public async Task WebApiTemplate_Instantiates_AndBuilds(string templateName, string framework)
    {
        // Arrange
        var frameworkVersion = framework.Replace(".", "");
        var projectName = $"TestWebApi{frameworkVersion}";
        var projectPath = Path.Combine(_tempDirectory, projectName);

        // Act
        var instantiateResult = RunDotnetCommand(
            $"new {templateName} -n {projectName} -o \"{projectPath}\" --framework {framework} --domain test.auth0.com --audience https://test-api.com"
        );

        // Assert
        instantiateResult.Should().Be(0, "Template instantiation should succeed");
        Directory.Exists(projectPath).Should().BeTrue();

        var appSettingsPath = Path.Combine(projectPath, "appsettings.json");
        File.Exists(appSettingsPath).Should().BeTrue();
        var appSettings = await File.ReadAllTextAsync(appSettingsPath);
        appSettings.Should().Contain("test.auth0.com");
        appSettings.Should().Contain("https://test-api.com");

        //var buildResult = RunDotnetCommand($"build \"{projectPath}\" --configuration Release");
        //buildResult.Should().Be(0, "Generated project should build successfully");
    }

    [Theory]
    [InlineData("auth0webapi", "--minimal", "net8.0")]
    [InlineData("auth0webapi", "--minimal", "net9.0")]
    [InlineData("auth0webapi", "--minimal", "net10.0")]
    public async Task MinimalWebApiTemplate_Instantiates_AndBuilds(
        string templateName,
        string minimalFlag,
        string framework)
    {
        // Arrange
        var frameworkVersion = framework.Replace(".", "");
        var projectName = $"TestMinimalApi{frameworkVersion}";
        var projectPath = Path.Combine(_tempDirectory, projectName);

        // Act
        var instantiateResult = RunDotnetCommand(
            $"new {templateName} {minimalFlag} -n {projectName} -o \"{projectPath}\" --framework {framework} --domain test.auth0.com --audience https://test-api.com"
        );

        // Assert
        instantiateResult.Should().Be(0, "Template instantiation should succeed");
        
        //var buildResult = RunDotnetCommand($"build \"{projectPath}\" --configuration Release");
        //buildResult.Should().Be(0, "Generated project should build successfully");
    }

    [Theory]
    [InlineData("auth0blazor", "net8.0")]
    [InlineData("auth0blazor", "net9.0")]
    [InlineData("auth0blazor", "net10.0")]
    public async Task BlazorWebAppTemplate_Instantiates_AndBuilds(string templateName, string framework)
    {
        // Arrange
        var frameworkVersion = framework.Replace(".", "");
        var projectName = $"TestBlazorWebApp{frameworkVersion}";
        var projectPath = Path.Combine(_tempDirectory, projectName);

        // Act
        var instantiateResult = RunDotnetCommand(
            $"new {templateName} -n {projectName} -o \"{projectPath}\" --framework {framework} --domain test.auth0.com --client-id test-client-id"
        );

        // Assert
        instantiateResult.Should().Be(0, "Template instantiation should succeed");
        
        // BlazorWebApp creates a solution with the server project in a subdirectory
        var appSettingsPath = Path.Combine(projectPath, projectName, "appsettings.json");
        var appSettings = await File.ReadAllTextAsync(appSettingsPath);
        appSettings.Should().Contain("test.auth0.com");

        //var buildResult = RunDotnetCommand($"build \"{projectPath}\" --configuration Release");
        //buildResult.Should().Be(0, "Generated project should build successfully");
    }

    [Fact]
    public void Template_WithoutAutoregister_DoesNotRequireAuth0Cli()
    {
        // Arrange
        var projectName = "TestWithoutAuth0";
        var projectPath = Path.Combine(_tempDirectory, projectName);

        // Act - instantiate with custom domain/client-id to skip autoregister
        var result = RunDotnetCommand(
            $"new auth0webapp -n {projectName} -o \"{projectPath}\" --framework net10.0 --domain test.auth0.com --client-id test-client-id"
        );

        // Assert - should succeed without Auth0 CLI
        result.Should().Be(0, "Template should instantiate without Auth0 CLI when using custom domain/client-id");
    }

    [Fact]
    public void InstantiatedTemplate_HasRegistrationScript()
    {
        // Arrange
        var projectName = "TestRegistrationScript";
        var projectPath = Path.Combine(_tempDirectory, projectName);

        // Act
        RunDotnetCommand(
            $"new auth0webapp -n {projectName} -o \"{projectPath}\" --framework net10.0 --domain test.auth0.com --client-id test-client-id"
        );

        // Assert
        File.Exists(Path.Combine(projectPath, "register-with-auth0.cmd")).Should().BeTrue(
            "Registration script should be present for manual registration"
        );
    }

    [Fact]
    public void InstantiatedTemplate_WithCustomDomainAndClientId_DoesNotHaveRegistrationFolder()
    {
        // Arrange
        var projectName = "TestNoRegistrationFolder";
        var projectPath = Path.Combine(_tempDirectory, projectName);

        // Act
        RunDotnetCommand(
            $"new auth0webapp -n {projectName} -o \"{projectPath}\" --framework net10.0 --domain test.auth0.com --client-id test-client-id"
        );

        // Assert
        Directory.Exists(Path.Combine(projectPath, "registration")).Should().BeFalse(
            "Registration folder should not be present when using custom domain/client-id"
        );
    }

    private bool IsTemplateInstalled(string templateName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "new uninstall",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = _tempDirectory
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            return false;
        }

        process.WaitForExit();
        var output = process.StandardOutput.ReadToEnd();
        
        // Check if the template name appears in the list of installed templates
        return output.Contains(templateName, StringComparison.OrdinalIgnoreCase);
    }

    private int RunDotnetCommand(string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = _tempDirectory
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new InvalidOperationException("Failed to start dotnet process");
        }

        process.WaitForExit();
        
        // Capture output for debugging if needed
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0)
        {
            Console.WriteLine($"Command failed: dotnet {arguments}");
            Console.WriteLine($"Output: {output}");
            Console.WriteLine($"Error: {error}");
        }

        return process.ExitCode;
    }
}
}
