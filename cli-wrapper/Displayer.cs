using System;

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
    if (registrationData.signing_keys is { Length: > 0 } && !string.IsNullOrEmpty(registrationData.signing_keys[0].subject))
    {
      Console.WriteLine($"Auth0 domain: {registrationData.signing_keys[0].subject.Replace("/CN=", "")}");
    }
    if (!string.IsNullOrEmpty(registrationData.client_id))
    {
      Console.WriteLine($"Client ID: {registrationData.client_id}");
    }
    if (!string.IsNullOrEmpty(registrationData.identifier))
    {
      Console.WriteLine($"Audience: {registrationData.identifier}");
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

  public static void DisplayErrorVerbose(string text)
  {
    if (Verbose) DisplayError(text);
  }

  public static void DisplayError(string text)
  {
    Console.WriteLine("ERROR: ************");
    Console.WriteLine($@"{text}");
    Console.WriteLine("*******************");
  }
}
