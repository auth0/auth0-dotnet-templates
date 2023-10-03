public static class Displayer
{
  public static bool Verbose { get; set; }

  public static void DisplayVerbose(string text)
  {
    if (Verbose)
    {
      Console.WriteLine(text);
    }
  }

  public static void DisplayRegistrationData(RegistrationData registrationData)
  {
    if (registrationData.signing_keys != null && registrationData.signing_keys.Length > 0 && registrationData.signing_keys[0].subject != null)
    {
      Console.WriteLine($@"Auth0 domain: {registrationData.signing_keys[0].subject.Replace("/CN=", "")}");
    }
    if (registrationData.client_id != null)
    {
      Console.WriteLine($@"Client ID: {registrationData.client_id}");
    }
    if (registrationData.identifier != null)
    {
      Console.WriteLine($@"Audience: {registrationData.identifier}");
    }
  }

  public static void DisplayTemplateConfiguration(string text)
  {
    if (Verbose)
    {
      Console.WriteLine("Template configuration: ---------");
      Console.WriteLine($@"{text}");
      Console.WriteLine("---------------------------------");
    }
  }

  public static void DisplayAppSettings(string text)
  {
    if (Verbose)
    {
      Console.WriteLine("Appsettings content: ---------");
      Console.WriteLine($@"{text}");
      Console.WriteLine("---------------------------------");
    }
  }

  public static void DisplayNewAppSettings(string text)
  {
    if (Verbose)
    {
      Console.WriteLine("New appsettings content: ---------");
      Console.WriteLine($@"{text}");
      Console.WriteLine("---------------------------------");
    }
  }

  public static void DisplayCommandOutput(string text)
  {
    if (Verbose)
    {
      Console.WriteLine("Command output: ---------");
      Console.WriteLine($@"{text}");
      Console.WriteLine("---------------------------------");
    }
  }
}
