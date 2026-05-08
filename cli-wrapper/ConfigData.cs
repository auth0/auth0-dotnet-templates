using System;

public class ConfigData
{
  public string AppName { get; set; } = string.Empty;
  public string AppDescription { get; set; } = string.Empty;
  public string AppType { get; set; } = string.Empty;
  public string Callbacks { get; set; } = string.Empty;
  public string LogoutUrls { get; set; } = string.Empty;
  public string[] AppSettingsFiles { get; set; } = Array.Empty<string>();
  public string RegistrationScriptFile { get; set; } = string.Empty;
  public bool Verbose { get; set; }
}
