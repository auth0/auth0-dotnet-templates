## CLI Wrapper

This project is a console application that acts as a wrapper for the Auth0 CLI to register the application with Auth0 during the creation step of an application from the template.

It leverages the `auth0 apps create` command (see [the docs)](https://auth0.github.io/auth0-cli/auth0_apps_create.html).

It requires Auth0 CLI version 1.0.1 or later.



## Template integration

- Each template must have a `registration` folder

- The `registration` folder must contain:

  - a copy of `cli-wrapper.dll`

  - a copy of `config.json` opportunely configured. For example:
    ```json
    {
      "AppName": "Auth0BlazorServer",
      "AppDescription": "Auth0BlazorServer app",
      "AppType": "regular",
      "Callbacks": "https://localhost:5001/callback",
      "LogoutUrls": "https://localhost:5001/",
      "AppSettingsFiles": ["./appsettings.json"]
    }
    ```

    

    - `AppName` must be the same as the one used in the template (`sourceName` property in `template.config` file)
    - `AppDescription` is the application description that will be stored in the Auth0 dashboard
    - `AppType` must contain the specific application type for the current project (see [type flag](https://auth0.github.io/auth0-cli/auth0_apps_create.html#flags) required by the `auth0 apps create` command of the Auth0 CLI)
    - `Callbacks` and `LogoutUrls` must use the same ports as in the templates (see `Properties/launchSettings.json`)
    - `AppSettingsFiles` must point to the project's configuration files to update

- Each template must have the following `postAction` in its `template.config` file:
  ```json
    "postActions": [{
      "actionId": "3A7C4B45-1F5D-4A30-959A-51B88E82B5D2",
      "args": {
        "executable": "dotnet",
        "args": "./registration/cli-wrapper.dll",
        "redirectStandardOutput": false,
        "redirectStandardError": false
      },
      "manualInstructions": [{
         "text": "Run 'dotnet ./registration/cli-wrapper.dll'"
      }],
      "continueOnError": false,
      "description ": "Register your application with Auth0"
    }]
  ```

  

- After the project has been created, the `registration` folder must be removed (TBD) (No [predefined Post action](https://github.com/dotnet/templating/wiki/Post-Action-Registry)?)



## Release

To publish the application, run the following command:

```bash
dotnet publish -c Release -p:UseAppHost=false
```



## TODO

- Remove the `registration` folder
- Verbose mode
- Avoid references to the `registration` folder within the code?
- Try/catch
- Unit tests
- Add the CLI wrapper to other templates
- Automate the addition of the CLI wrapper at build time