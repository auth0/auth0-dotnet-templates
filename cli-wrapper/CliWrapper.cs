﻿using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

public class CliWrapper
{
  ConfigData configData = new ConfigData();

  public CliWrapper()
  { }

  public bool Verbose { get; set; }

  public async Task<bool> IsAuth0CliInstalled()
  {
    string cmdOutput = await RunCommand("auth0", "--version");
    string currentCliVersionString = cmdOutput.Split(" ")[2];
    Version currentCliVersion = new Version(currentCliVersionString);

    Displayer.DisplayVerbose($@"Auth0 CLI version: {currentCliVersionString}");

    return currentCliVersion.CompareTo(new Version("1.0.1")) >= 0;
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
      Console.WriteLine("ERROR: ************");
      Console.WriteLine($@"{ex.Message}");
      Console.WriteLine("*******************");
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

  public void UpdateConfigFiles(RegistrationData registrationData)
  {
    foreach (var settingsFile in configData.AppSettingsFiles)
    {
      Displayer.DisplayVerbose($@"Reading settings from {settingsFile}");

      string text = File.ReadAllText($@"{settingsFile}");

      Displayer.DisplayAppSettings(text);

      if (registrationData.signing_keys != null && registrationData.signing_keys.Length >0 && registrationData.signing_keys[0].subject != null)
      {
        text = text.Replace("yourdomain.auth0.com", (registrationData.signing_keys[0].subject).Replace("/CN=", ""));
      }
      if (registrationData.client_id != null)
      {
        text = text.Replace("your-client-id", registrationData.client_id);
      }
      if (registrationData.identifier != null)
      {
        text = text.Replace("https://your-api-id.com", registrationData.identifier);
      }

      Displayer.DisplayNewAppSettings(text);

      File.WriteAllText($@"{settingsFile}", text);
    }
  }

  public void RemoveRegistrationFolder()
  {
    string registrationFolderPath = $@"{Path.Combine(System.AppContext.BaseDirectory)}";

    Displayer.DisplayVerbose($@"About to remove the folder {registrationFolderPath}");

    Directory.Delete(registrationFolderPath, true);
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

  private string CreateAudience(string appName)
  {
    Displayer.DisplayVerbose($@"Application name to tranform into audience: {appName}");

    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
    appName = rgx.Replace(appName, "-");

    var audience = $@"https://{appName.ToLower()}.com";

    Displayer.DisplayVerbose($@"Resulting audience: {audience}");

    return audience;
  }
}