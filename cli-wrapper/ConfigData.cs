public class ConfigData
{
  public string? AppName { get; set; }
  public string? AppDescription { get; set; }
  public string? AppType { get; set; }
  public string? Callbacks { get; set; }
  public string? LogoutUrls { get; set; }
  public string[]? AppSettingsFiles {get; set;}

  public bool Verbose {get; set;}
}
