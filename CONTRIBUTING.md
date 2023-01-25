# Contribution

Please read [Auth0's contribution guidelines](https://github.com/auth0/open-source-template/blob/master/GENERAL-CONTRIBUTING.md).

## Environment setup

- Make sure you have the [.NET  SDK](https://dotnet.microsoft.com/download/dotnet/7.0) installed.
- Follow the local development steps below to get started.

## Local development

- `dotnet pack`: create a local NuGet package in the `./bin/Debug/` folder.
- `dotnet new -i ./bin/Debug/Auth0.Templates.2.0.0-alpha.nupkg`: install the local NuGet package
- `npm run pack-install`: if you have Node.js installed, it installs or updates the local NuGet package

## Testing

Currently there are no automatic tests.

Manual tests should be run on the .NET CLI, Visual Studio 2022 for Windows, and Visual Studio 2022 for Mac.