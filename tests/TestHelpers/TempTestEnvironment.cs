using System;
using System.IO;
using System.Text.Json;

/// <summary>
/// Helper class for creating temporary test directories and config files.
/// </summary>
public class TempTestEnvironment : IDisposable
{
    public string TempDirectory { get; }
    public string ConfigFilePath { get; }

    public TempTestEnvironment(ConfigData? configData = null)
    {
        TempDirectory = Path.Combine(Path.GetTempPath(), $"auth0-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(TempDirectory);

        ConfigFilePath = Path.Combine(TempDirectory, "config.json");
        
        // Create a default config if none provided
        configData ??= new ConfigData
        {
            AppName = "TestApp",
            AppDescription = "Test Application",
            AppType = "regular",
            Callbacks = "https://localhost:5001/callback",
            LogoutUrls = "https://localhost:5001/",
            AppSettingsFiles = new[] { Path.Combine(TempDirectory, "appsettings.json") },
            RegistrationScriptFile = "../register-with-auth0.cmd",
            Verbose = false
        };

        File.WriteAllText(ConfigFilePath, JsonSerializer.Serialize(configData));
    }

    public string CreateAppSettingsFile(string content = "")
    {
        if (string.IsNullOrEmpty(content))
        {
            content = @"{
  ""Auth0"": {
    ""Domain"": ""yourdomain.auth0.com"",
    ""ClientId"": ""your-client-id""
  }
}";
        }

        var filePath = Path.Combine(TempDirectory, "appsettings.json");
        File.WriteAllText(filePath, content);
        return filePath;
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(TempDirectory))
            {
                Directory.Delete(TempDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors in tests
        }
    }
}
