
var cliWrapper = new CliWrapper();

//Console.WriteLine($@"Environment.CurrentDirectory: {Environment.CurrentDirectory}");
//Console.WriteLine($@"System.Reflection.Assembly.GetExecutingAssembly().Location: {System.Reflection.Assembly.GetExecutingAssembly().Location}");
//Console.WriteLine($@"System.AppContext.BaseDirectory: {System.AppContext.BaseDirectory}");

if (await cliWrapper.IsAuth0CliInstalled())
{
  Console.WriteLine("Auth0 CLI is installed.");

  var registrationData = await cliWrapper.Register();
  Console.WriteLine(registrationData.client_id??"");
  Console.WriteLine(registrationData.identifier ?? "");
  if (registrationData.signing_keys != null)
  {
    Console.WriteLine((registrationData.signing_keys[0].subject ?? "").Replace("/CN=", ""));
  }

  cliWrapper.UpdateConfigFiles(registrationData);
} else
{
  Console.WriteLine("Auth0 CLI is not installed or it's not the required version.");
  Console.WriteLine("Please, install the Auth0 CLI ver. 1.0.1 or later (https://auth0.github.io/auth0-cli/).");
}

cliWrapper.RemoveRegistrationFolder();
