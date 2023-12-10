## CLI Wrapper

This project is a console application that acts as a wrapper for the Auth0 CLI to register the application with Auth0 during the creation step of an application from the template.

It leverages the `auth0 apps create` and `auth0 apis create` commands (see [the docs)](https://auth0.github.io/auth0-cli/#available-commands).

It requires Auth0 CLI version 1.0.1 or later.

## Template integration

In order to enable a template to automatic registration via the CLI Wrapper, apply the following:

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
    "Verbose": false
  }
  ```

  

  - `AppName` must be the same as the one used in the template (`sourceName` property in `template.config` file)
  - `AppDescription` is the application description that will be stored in the Auth0 dashboard
  - `AppType` must contain the specific application type for the current project (see [type flag](https://auth0.github.io/auth0-cli/auth0_apps_create.html#flags) required by the `auth0 apps create` command of the Auth0 CLI). Its value is `api` for API templates.
  - `Callbacks` and `LogoutUrls` must use the same ports as in the templates (see `Properties/launchSettings.json`). Set these properties to empty strings for API templates.
  - `AppSettingsFiles` must point to the project's configuration files to update after registration
  - `Verbose` is a boolean setting that enables a verbose onscreen log for diagnostic purposes.

- Add the following `postAction` in the `template.config` file of the template:
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

- Add a `<Copy>` element to the `Auth0Templates.csproj` file specifying the `registration` folder path, as in the following example:

  ```xml
  <Project Sdk="Microsoft.NET.Sdk">
  
    <!-- Other markup -->
    
    <Target Name="CopyCliWrapper" AfterTargets="BeforeBuild">
      <ItemGroup>
        <CliWrapperFiles Include="cli-wrapper\bin\Release\net7.0\publish\cli-wrapper.*"/>
      </ItemGroup>
  
      <Copy SourceFiles="@(CliWrapperFiles)" DestinationFolder="templates\Auth0.BlazorServer\registration" SkipUnchangedFiles="false" />
      <Copy SourceFiles="@(CliWrapperFiles)" DestinationFolder="templates\Auth0.WebAPI\registration" SkipUnchangedFiles="false" />
    </Target>
  </Project>
  ```

  

## Build-Time Behavior

At the package build-time:

- the CLI Wrapper is compiled and published as a framework dependent assembly (OS independent).

  The following command is used to compile the application:

  ```bash
  dotnet publish -c Release -p:UseAppHost=false
  ```

- A copy of the CLI Wrapper assembly and related files are copied to the `registration` folder of each template

- The NuGet package is created

