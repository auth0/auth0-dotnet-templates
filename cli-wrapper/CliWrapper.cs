using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class CliWrapper
{
  private ConfigData configData = new();
  private Version? cliVersion;
  private readonly IProcessExecutor processExecutor;

  public CliWrapper() : this(new ProcessExecutor())
  {
  }

  public CliWrapper(IProcessExecutor processExecutor)
  {
    this.processExecutor = processExecutor;
    
    string templateConfigurationFilePath = Path.Combine(AppContext.BaseDirectory, "config.json");
    string text = File.ReadAllText(templateConfigurationFilePath);

    configData = JsonSerializer.Deserialize<ConfigData>(text) ?? new();

    Displayer.Verbose = configData.Verbose;
  }

  public async Task<bool> IsAuth0CliInstalled()
  {
    try
    {
      string cmdOutput = await processExecutor.RunCommandAsync("auth0", "--version");
      string currentCliVersionString = cmdOutput.Split(' ')[2];
      cliVersion = new Version(currentCliVersionString);

      Displayer.DisplayVerbose($"Auth0 CLI version: {currentCliVersionString}");

      return cliVersion.CompareTo(new Version("1.0.1")) >= 0;
    }
    catch (Exception ex)
    {
      Displayer.DisplayErrorVerbose(ex.Message);
      return false;
    }
  }

  public async Task<RegistrationData> Register()
  {
    var registrationData = new RegistrationData("", "", "", new[] { new SigningKeys("") });
    var registrationOutputText = "";

    try
    {
      if (configData.AppType.ToLower() == "api")
      {
        registrationOutputText = await processExecutor.RunCommandAsync("auth0", $"apis create --name {configData.AppName} --identifier {CreateAudience(configData.AppName)} --no-input --json");
        Console.WriteLine("API registered!");
      }
      else
      {
        registrationOutputText = await processExecutor.RunCommandAsync("auth0", $"apps create --name {configData.AppName} --description \"{configData.AppDescription}\" --type {configData.AppType} --callbacks \"{configData.Callbacks}\" --logout-urls \"{configData.LogoutUrls}\" --no-input --reveal-secrets --json");
        Console.WriteLine("Application registered!");
      }
      
    } catch (Exception ex)
    {
      Displayer.DisplayError(ex.Message);
    }

    try
    {
      registrationData = JsonSerializer.Deserialize<RegistrationData>(registrationOutputText) ?? registrationData;
    }
    catch (Exception ex)
    {
      Console.WriteLine("ERROR: ************");
      Console.WriteLine($"{ex.Message}");
      Console.WriteLine();
      Console.WriteLine("Registration output: -------");
      Console.WriteLine($"{registrationOutputText}");
      Console.WriteLine("*******************");
    }

    return registrationData;
  }

  public async Task UpdateConfigFiles(RegistrationData registrationData)
  {
    foreach (var settingsFile in configData.AppSettingsFiles)
    {
      Displayer.DisplayVerbose($"Reading settings from {settingsFile}");

      string text = await File.ReadAllTextAsync(settingsFile);

      Displayer.DisplayAppSettings(text);

      string currentDomain = await GetCurrentDomain(registrationData);

      if (!string.IsNullOrEmpty(currentDomain))
      {
        text = text.Replace("yourdomain.auth0.com", currentDomain);
      }

      if (!string.IsNullOrEmpty(registrationData.client_id))
      {
        text = text.Replace("your-client-id", registrationData.client_id);
      }
      if (!string.IsNullOrEmpty(registrationData.identifier))
      {
        text = text.Replace("https://your-api-id.com", registrationData.identifier);
      }

      Displayer.DisplayNewAppSettings(text);

      await File.WriteAllTextAsync(settingsFile, text);
    }
  }

  public Task RemoveRegistrationFolder()
  {
    return Task.Run(() => {
      string registrationFolderPath = AppContext.BaseDirectory;

      Displayer.DisplayVerbose($"About to remove the folder {registrationFolderPath}");

      if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
      {
        Directory.Delete(registrationFolderPath, true);
      } 
      else if (OperatingSystem.IsWindows())
      {
        // Hack to bypass executable lock in Windows (https://andreasrohner.at/posts/Programming/C%23/A-platform-independent-way-for-a-C%23-program-to-update-itself/)
        var delScriptName = Path.Combine(registrationFolderPath, "selfdel.bat");

        using var batFile = new StreamWriter(delScriptName);
        batFile.WriteLine("@ECHO OFF");
        batFile.WriteLine("TIMEOUT /t 5 /nobreak > NUL");
        batFile.WriteLine($"RMDIR /S /Q {registrationFolderPath}");
        batFile.Close();

        processExecutor.RunCommandAndForget(delScriptName, "");
      }
    });
  }

  internal string CreateAudience(string appName)
  {
    Displayer.DisplayVerbose($"Application name to transform into audience: {appName}");

    Regex rgx = new("[^a-zA-Z0-9 -]");
    appName = rgx.Replace(appName, "-");
    
    // Replace spaces and multiple consecutive dashes with single dash
    appName = Regex.Replace(appName, @"[\s-]+", "-");

    var audience = $"https://{appName.ToLower()}.com";

    Displayer.DisplayVerbose($"Resulting audience: {audience}");

    return audience;
  }

  private async Task<string> GetCurrentDomain(RegistrationData registrationData)
  {
    string currentDomain = "";

    // Use new tenant list command for CLI version 1.17.0 and above
    if (cliVersion is not null && cliVersion.CompareTo(new Version("1.17.0")) >= 0)
    {
      string tenantListText = await processExecutor.RunCommandAsync("auth0", "tenants list --json");

      try
      {
        var tenantList = JsonSerializer.Deserialize<TenantData[]>(tenantListText);

        if (tenantList is not null)
        {
          Displayer.DisplayVerbose($"Found {tenantList.Length} tenants.");

          // Find the active tenant
          var activeTenant = Array.Find(tenantList, tenant => tenant.active);
          
          if (activeTenant is not null && !string.IsNullOrEmpty(activeTenant.name))
          {
            Displayer.DisplayVerbose($"Active tenant: {activeTenant.name}");
            currentDomain = activeTenant.name;
          }
        }
      }
      catch (Exception ex)
      {
        Displayer.DisplayError(ex.Message);
      }
    }
    else
    {
      // For older CLI versions, try to get domain from registration data first
      if (registrationData.signing_keys is not null && registrationData.signing_keys.Length > 0 && !string.IsNullOrEmpty(registrationData.signing_keys[0].subject))
      {
        Displayer.DisplayVerbose($"Certificate subject: {registrationData.signing_keys[0].subject}");
        currentDomain = registrationData.signing_keys[0].subject.Replace("/CN=", "");
      } 
      else
      {
        // Fallback to apis list method
        currentDomain = await GetCurrentDomain();
      }
    }

    Displayer.DisplayVerbose($"Current domain: {currentDomain}");

    return currentDomain;
  }

  private async Task<string> GetCurrentDomain()
  {
    string currentDomain = "";
    
    // This method is only called for CLI versions below 1.17.0
    string registrationDataListText = await processExecutor.RunCommandAsync("auth0", "apis list -n 1 --json");

    try
    {
      var registrationDataList = JsonSerializer.Deserialize<RegistrationData[]>(registrationDataListText);

      if (registrationDataList is not null)
      {
        Displayer.DisplayVerbose($"Found {registrationDataList.Length} registered APIs.");

        if (registrationDataList.Length > 0)
        {
          var registrationData = registrationDataList[0];
          if (!string.IsNullOrEmpty(registrationData.identifier))
          {
            Displayer.DisplayVerbose($"System API identifier: {registrationData.identifier}");
            currentDomain = new Uri(registrationData.identifier).Host;
          }
        }
      }
    }
    catch (Exception ex)
    {
      Displayer.DisplayError(ex.Message);
    }
    
    return currentDomain;
  }
}