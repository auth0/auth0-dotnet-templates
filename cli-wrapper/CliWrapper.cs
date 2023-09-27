﻿using System;
using System.Diagnostics;
using System.Text.Json;

public class CliWrapper
{
  ConfigData configData = new ConfigData();
  public CliWrapper()
  {
  }

  public async Task<bool> IsAuth0CliInstalled()
  {
    string cmdOutput = await RunCommand("auth0", "--version");
    string currentCliVersionString = cmdOutput.Split(" ")[2];
    Version currentCliVersion = new Version(currentCliVersionString);

    return currentCliVersion.CompareTo(new Version("1.0.1")) >= 0;
  }

  public async Task<RegistrationData> Register()
  {
    var registrationData = new RegistrationData("", "", new SigningKeys[] {new SigningKeys("")});

    try
    {
      string text = File.ReadAllText(@"./registration/config.json");
      configData = JsonSerializer.Deserialize<ConfigData>(text);

      string registrationOutputText = await RunCommand("auth0", $"apps create --name {configData.AppName} --description \"{configData.AppDescription}\" --type {configData.AppType} --callbacks \"{configData.Callbacks}\" --logout-urls \"{configData.LogoutUrls}\" --no-input --reveal-secrets --json");
      Console.WriteLine("Application registered!");

      registrationData = JsonSerializer.Deserialize<RegistrationData>(registrationOutputText)!;
    } catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
    }

    return registrationData;
  }

  public void UpdateConfigFiles(RegistrationData registrationData)
  {
    foreach (var settingsFile in configData.AppSettingsFiles)
    {
      string text = File.ReadAllText($@"{settingsFile}");

      text = text.Replace("yourdomain.auth0.com", registrationData.signing_keys[0].subject.Replace("/CN=", ""));
      text = text.Replace("your-client-id", registrationData.client_id);

      File.WriteAllText($@"{settingsFile}", text);
    }
  }

  public void RemoveRegistrationFolder()
  {
    Directory.Delete(@"./registration", true);
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
    string result = "";

    try
    {
      var proc = Process.Start(startInfo);
      ArgumentNullException.ThrowIfNull(proc);
      string output = proc.StandardOutput.ReadToEnd();
      await proc.WaitForExitAsync();

      //TODO: Capture errors

      result = output;
    }
    catch (SystemException ex)
    {
      Console.WriteLine(ex.Message);
    }

    return result;
  }
}

