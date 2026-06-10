---
name: registration-support
description: Add the scripts to register the application with Auth0, configure the template accordingly, and adapt the build process.
---
# Instructions

In order to enable a template to automatic registration via the CLI Wrapper, apply the following instructions.

## Check User Input

If the user provides the folder of the template to configure, focus on on that template.

If the user does noot provide any input, ask them to select the template they want to configure from a list of all the available templates. You can get the list of templates by scanning the `templates` folder and retrieving the names of the subfolders, which correspond to the template names.

## Check Multiproject Templates

IMPORTANT: If the template contains multiple projects, make sure to follow this approach:

- Identify which projects need registrations with Auth0. You can check if they include configuration files with Auth0 settings (e.g., `appsettings.json` with `Auth0:Domain` and `Auth0:ClientId` properties) or if they contain code that uses Auth0 configuration values.

- For each project that requires registration, add a `registration` folder with a `config.json` file properly configured (see next section).

- Create one single registration script (e.g., `register-with-auth0.cmd`) that will be used for all projects. This script will be responsible for registering all the necessary applications with Auth0. Make sure to place this script in a common folder (e.g., the template's root folder) and set the `RegistrationScriptFile` property in each `config.json` file to point to this script.


## Registration Script Setup

- Create a `registration` folder in the template's folder.

- Add a `config.json` file to the `registration` folder properly configured. For example:

  ```json
  {
    "AppName": "Auth0BlazorServer",
    "AppDescription": "Blazor Server application created and registered via Auth0 Templates for .NET",
    "AppType": "regular",
    "Callbacks": "https://localhost:5001/callback",
    "LogoutUrls": "https://localhost:5001/",
    "AppSettingsFiles": ["./appsettings.json"],
    "RegistrationScriptFile": "../register-with-auth0.cmd",
    "Verbose": false
  }
  ```  

  - `AppName` must be the same as the one used in the template (`sourceName` property in `template.config` file)
  - `AppDescription` is the application description that will be stored in the Auth0 dashboard
  - `AppType` must contain the specific application type for the current project (see [type flag](https://auth0.github.io/auth0-cli/auth0_apps_create.html#flags) required by the `auth0 apps create` command of the Auth0 CLI). Its value is `api` for API templates.
  - `Callbacks` and `LogoutUrls` must use the same ports as in the templates (see `Properties/launchSettings.json`). Set these properties to empty strings for API templates.
  - `AppSettingsFiles` must point to the project's configuration files to update after registration
  - `RegistrationScriptFile` is the relative path of the script `register-with-auth0.cmd` with respect to the `registration` folder . This path is used during the registration folder removal. Make sure it matches the actual file location.
  - `Verbose` is a boolean setting that enables a verbose onscreen log for diagnostic purposes.


## Registration Script

- Add a file named `register-with-auth0.cmd` to the root folder of the template with the following content:

  ```bash
  rem #if (OS != "Windows_NT")
  #!/bin/sh
  dotnet ./registration/cli-wrapper.dll
  rem #else
  dotnet registration/cli-wrapper.dll
  rem #endif
  ```

## Template Configuration

- If the `autoregister` computed symbol is not defined in the `template.config` file, define it as follows:

  ```json
      "autoregister": {
      "type": "computed",
      "value": "(domain == \"yourdomain.auth0.com\" && clientId == \"your-client-id\")"
    }
  ```
- Add the following `postAction` to the `template.config` file of the template:

  ```json
    "postActions": [{
      "condition": "(OS != \"Windows_NT\")",
      "description": "Make scripts executable",
      "manualInstructions": [{
        "text": "Run 'chmod +x register-with-auth0.cmd'"
      }],
      "actionId": "cb9a6cf3-4f5c-4860-b9d2-03a574959774",
      "args": {
        "+x": "register-with-auth0.cmd"
      },
      "continueOnError": true
    },
      {
      "actionId": "3A7C4B45-1F5D-4A30-959A-51B88E82B5D2",
      "condition": "(autoregister)",
      "args": {
        "executable": "register-with-auth0.cmd",
        "redirectStandardOutput": false,
        "redirectStandardError": false
      },
      "manualInstructions": [{
         "text": "To register your app with Auth0, run './register-with-auth0.cmd'"
      }],
      "continueOnError": false,
      "description ": "Register your application with Auth0"
    }]
  ```
  
## Build Configuration

- Add a `<Copy>` element to the `Auth0Templates.csproj` file specifying the `registration` folder path, as in the following example:

  ```xml
  <Project Sdk="Microsoft.NET.Sdk">
  
    <!-- Other markup -->
    
    <Target Name="AddCliWrapper" AfterTargets="BeforeBuild">
      <ItemGroup>
        <CliWrapperFiles Include="cli-wrapper\bin\Release\net7.0\publish\cli-wrapper.*"/>
      </ItemGroup>
  
      <Exec Command="dotnet publish cli-wrapper/cli-wrapper.csproj -c Release -p:UseAppHost=false" />
      
      <Copy SourceFiles="@(CliWrapperFiles)" DestinationFolder="templates\Auth0.BlazorServer\registration" SkipUnchangedFiles="false" />
      <Copy SourceFiles="@(CliWrapperFiles)" DestinationFolder="templates\Auth0.WebAPI\registration" SkipUnchangedFiles="false" />
    </Target>
  </Project>
  ```

  
  