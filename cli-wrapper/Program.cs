var cliWrapper = new CliWrapper();

//Displayer.Verbose = true;

if (await cliWrapper.IsAuth0CliInstalled())
{
  Console.WriteLine("Auth0 CLI is installed.");

  var registrationData = await cliWrapper.Register();

  Displayer.DisplayRegistrationData(registrationData);
  
  await cliWrapper.UpdateConfigFiles(registrationData);
} else
{
  Console.WriteLine("Auth0 CLI is not installed or it's not the required version.");
  Console.WriteLine("Please, install the Auth0 CLI ver. 1.0.1 or later (https://auth0.github.io/auth0-cli/).");
}

await cliWrapper.RemoveRegistrationFolder();
