using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var cliWrapper = new CliWrapper();

        //Displayer.Verbose = true;

        if (await cliWrapper.IsAuth0CliInstalled())
        {
            Console.WriteLine("Auth0 CLI is installed.");
            Console.WriteLine("Before starting the registration step, make sure you are logged in to your Auth0 tenant through Auth0 CLI.");
            Console.WriteLine("If you are not logged in, open a new terminal window and run `auth0 login`, then come back here to continue the registration step.");
            Console.WriteLine("If you are already logged in, press any key to continue...");
            Console.ReadKey();

            var registrationData = await cliWrapper.Register();

            Displayer.DisplayRegistrationData(registrationData);
            
            await cliWrapper.UpdateConfigFiles(registrationData);
        }
        else
        {
            Console.WriteLine("Auth0 CLI is not installed or it's not the required version.");
            Console.WriteLine("Please, install the Auth0 CLI ver. 1.0.1 or later (https://auth0.github.io/auth0-cli/).");
        }

        await cliWrapper.RemoveRegistrationFolder();
    }
}
