using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

public class CliWrapper
{
  ConfigData configData = new ConfigData();

  public CliWrapper()
  { }

  public async Task<bool> IsAuth0CliInstalled()
  {
    bool result = false;

    try
    {
      string cmdOutput = await RunCommand("auth0", "--version");
      string currentCliVersionString = cmdOutput.Split(" ")[2];
      Version currentCliVersion = new Version(currentCliVersionString);

      Displayer.DisplayVerbose($@"Auth0 CLI version: {currentCliVersionString}");

      result = currentCliVersion.CompareTo(new Version("1.0.1")) >= 0;
    }
    catch (Exception ex)
    {
      Displayer.DisplayErrorVerbose(ex.Message);
    }

    return result;
  }

  public async Task<RegistrationData> Register()
  {
    var registrationData = new RegistrationData("", "", "", new SigningKeys[] {new SigningKeys("")});
    var registrationOutputText = "";

    try
    {
      string templateConfigurationFilePath = $@"{Path.Combine(System.AppContext.BaseDirectory, "config.json")}";

      Displayer.DisplayVerbose($@"Reading template configuration from {templateConfigurationFilePath}");

      string text = File.ReadAllText(templateConfigurationFilePath);

      Displayer.DisplayTemplateConfiguration(text);

      configData = JsonSerializer.Deserialize<ConfigData>(text);

      if (configData.AppType.ToLower() == "api")
      {
        registrationOutputText = await RunCommand("auth0", $"apis create --name {configData.AppName} --identifier {CreateAudience(configData.AppName)} --no-input --json");
        Console.WriteLine("API registered!");
      }
      else
      {
        registrationOutputText = await RunCommand("auth0", $"apps create --name {configData.AppName} --description \"{configData.AppDescription}\" --type {configData.AppType} --callbacks \"{configData.Callbacks}\" --logout-urls \"{configData.LogoutUrls}\" --no-input --reveal-secrets --json");
        Console.WriteLine("Application registered!");
      }
      
    } catch (Exception ex)
    {
      Displayer.DisplayError(ex.Message);
    }

    try
    {
      registrationData = JsonSerializer.Deserialize<RegistrationData>(registrationOutputText)!;
    }
    catch (Exception ex)
    {
      Console.WriteLine("ERROR: ************");
      Console.WriteLine($@"{ex.Message}");
      Console.WriteLine();
      Console.WriteLine("Registration output: -------");
      Console.WriteLine($@"{registrationOutputText}");
      Console.WriteLine("*******************");
    }

    return registrationData;
  }

  public async Task UpdateConfigFiles(RegistrationData registrationData)
  {
    foreach (var settingsFile in configData.AppSettingsFiles)
    {
      Displayer.DisplayVerbose($@"Reading settings from {settingsFile}");

      string text = File.ReadAllText($@"{settingsFile}");

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

      File.WriteAllText($@"{settingsFile}", text);
    }
  }

  public async Task RemoveRegistrationFolder()
  {
    string registrationFolderPath = $@"{Path.Combine(System.AppContext.BaseDirectory)}";

    Displayer.DisplayVerbose($@"About to remove the folder {registrationFolderPath}");

    if (Environment.OSVersion.Platform == PlatformID.Unix ||
          Environment.OSVersion.Platform == PlatformID.MacOSX)
    {
      Directory.Delete(registrationFolderPath, true);
    } else
    {
      // Hack to bypass executable lock in Windows (https://andreasrohner.at/posts/Programming/C%23/A-platform-independent-way-for-a-C%23-program-to-update-itself/)
      var delScriptName = Path.Combine(registrationFolderPath, "selfdel.bat");

      using (var batFile = new StreamWriter (File.Create (delScriptName))) {
        batFile.WriteLine ("@ECHO OFF");
        batFile.WriteLine ("TIMEOUT /t 5 /nobreak > NUL");
        batFile.WriteLine ($@"RMDIR /S /Q {registrationFolderPath}");
      }

      RunCommandAndForget(delScriptName, "");

      Environment.Exit(0);
    }
  }

  private async Task<string> RunCommand(string command, string args)
  {
    ProcessStartInfo startInfo = new()
    {
      FileName = command,
      Arguments = args,
      CreateNoWindow = true,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
    };

    Displayer.DisplayVerbose($@"About to run command: {command} {args}");

    var proc = Process.Start(startInfo);
    ArgumentNullException.ThrowIfNull(proc);
    string output = proc.StandardOutput.ReadToEnd();
    string errorText = proc.StandardError.ReadToEnd();
    await proc.WaitForExitAsync();

    if (!string.IsNullOrEmpty(errorText) && errorText.Contains("error"))
    {
      throw new Exception(errorText);
    }

    Displayer.DisplayCommandOutput(output);

    return output;
  }

  private void RunCommandAndForget(string command, string args)
  {
    ProcessStartInfo startInfo = new()
    {
      FileName = command,
      Arguments = args,
      CreateNoWindow = true,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
    };

    Displayer.DisplayVerbose($@"About to launch command: {command} {args}");

    var proc = Process.Start(startInfo);
  }

  private string CreateAudience(string appName)
  {
    Displayer.DisplayVerbose($@"Application name to tranform into audience: {appName}");

    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
    appName = rgx.Replace(appName, "-");

    var audience = $@"https://{appName.ToLower()}.com";

    Displayer.DisplayVerbose($@"Resulting audience: {audience}");

    return audience;
  }

  private async Task<string> GetCurrentDomain(RegistrationData registrationData)
  {
    string currentDomain = "";

    if (registrationData.signing_keys != null && registrationData.signing_keys.Length > 0 && !string.IsNullOrEmpty(registrationData.signing_keys[0].subject))
    {
      Displayer.DisplayVerbose($@"Certificate subject: {registrationData.signing_keys[0].subject}");
      currentDomain = registrationData.signing_keys[0].subject.Replace("/CN=", "");
    } else
    {
      currentDomain = await GetCurrentDomain();
    }

    Displayer.DisplayVerbose($@"Current domain: {currentDomain}");

    return currentDomain;
  }

  private async Task<string> GetCurrentDomain()
  {
    string currentDomain = "";
    string registrationDataListText = await RunCommand("auth0", "apis list -n 1 --json");

    try
    {
      var registrationDataList = JsonSerializer.Deserialize<RegistrationData[]>(registrationDataListText)!;

      Displayer.DisplayVerbose($@"Found {registrationDataList.Length} registered APIs.");

      if (registrationDataList.Length > 0)
      {
        var registrationData = registrationDataList[0];
        if (!string.IsNullOrEmpty(registrationData.identifier))
        {
          Displayer.DisplayVerbose($@"System API identifier: {registrationData.identifier}");
          currentDomain = new Uri(registrationData.identifier).Host;
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
