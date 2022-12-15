# Auth0 Templates for .NET
The Auth0 Templates for .NET package allows you to quickly bootstrap a .NET application secured with Auth0. You can use the Auth0 Templates with the .NET CLI and Visual Studio.

## Getting started

### Requirements

* An Auth0 account. You can [sign up for a free one](https://auth0.com/signup) right now.
* .NET Core 7.0 SDK or higher.
* (Optional) Visual Studio 2022 for Windows (ver. 17.4)

### Installation

You can install the Auth0 Templates from the NuGet repository or from the source code.

> :warning: The current version of Auth0.Templates are not published on NuGet yet.

To work locally with the Auth0 Templates, i.e., using the templates coming from the source code, execute the following steps:

1. Clone this repository

2. Create a local NuGet package by running the following command in the root folder of the project:

   ```bash
   dotnet pack
   ```

   Your NuGet package will be generated in the `./bin/Debug/` folder.

3. Install the templates by running the following command:

   ```bash
   dotnet new -i ./bin/Debug/Auth0.Templates.2.0.0-alpha.nupkg
   ```



:information_source: If you have Node.js installed on your machine, you can create and install a local NuGet package by using the following command in the root folder of the project:

```bash
npm run pack-install
```

That command also works for updating a currently installed package.

## Usage

Currently, the following .NET templates are implemented:

- [Auth0 MVC Application](docs/auth0webapp.md)
- [Auth0 Web API Application](docs/auth0webapi.md)
- [Auth0 Blazor Server Application](docs/auth0blazorserver.md)
- [Auth0 Blazor WebAssembly Application](docs/auth0blazorwasm.md)

## License

This project is licensed under the Apache license. See the [License.txt](License.txt) file for more info.

