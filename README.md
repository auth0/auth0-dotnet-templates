# Auth0 Templates for .NET
The Auth0 Templates for .NET package allows you to quickly bootstrap a .NET application secured with Auth0. You can use the Auth0 Templates with the .NET CLI and Visual Studio.

## Getting started

### Requirements

* An Auth0 account. You can [sign up for a free one](https://auth0.com/signup) right now.
* .NET Core 7.0 SDK or higher.
* (Optional, but recommended) [Auth0 CLI](https://github.com/auth0/auth0-cli) version 1.0.1
* (Optional) Visual Studio 2022 for Windows (ver. 17.4+)
* (Optional) Visual Studio 2022 for Mac (ver. 17.5+)
* (Optional) JetBrains Rider (ver. 2024.1+)

### Installation

You can install the Auth0 Templates from [NuGet](https://www.nuget.org/packages/Auth0.Templates/) by running the following command in your terminal window:

```bash
dotnet new install Auth0.Templates
```

Once installed, the templates are available in .NET CLI, Visual Studio for Windows, Visual Studio for Mac, and JetBrains Rider.

## Usage

Currently, the following .NET templates are implemented:

- [Auth0 MVC Application](docs/auth0webapp.md)
- [Auth0 Web API Application](docs/auth0webapi.md)
- [Auth0 Blazor Web App](docs/auth0blazor.md)
- [Auth0 Blazor Server Application](docs/auth0blazorserver.md)
- [Auth0 Blazor WebAssembly Application](docs/auth0blazorwasm.md)
- [Auth0 MAUI Application](docs/auth0maui.md)

## Installing from source code

To work locally with the Auth0 Templates, i.e., using the templates from the source code, follow these steps:

1. Clone this repository

2. Create a local NuGet package by running the following command in the root folder of the project:

   ```bash
   dotnet pack -o ./output
   ```

   Your NuGet package will be generated in the `./output/` folder.

3. Install the templates by running the following command:

   ```bash
   dotnet new -i ./output/Auth0.Templates.*.nupkg
   ```



> :information_source: If you have Node.js installed on your machine, you can create and install a local NuGet package by using the following command in the root folder of the project:
>
>```bash
>npm run pack-install
>```
>
> That command also works for updating a currently installed package.

## License

This project is licensed under the Apache license. See the [License.txt](License.txt) file for more info.

## Feedback
### Contributing

We appreciate feedback and contribution to this repo! Before you get started, please see the following:

- [Auth0's general contribution guidelines](https://github.com/auth0/open-source-template/blob/master/GENERAL-CONTRIBUTING.md)
- [Auth0's code of conduct guidelines](https://github.com/auth0/open-source-template/blob/master/CODE-OF-CONDUCT.md)
- [This repo's contribution guide](https://github.com/auth0/auth0-dotnet-templates/blob/main/CONTRIBUTING.md)

### Raise an issue

To provide feedback or report a bug, please [raise an issue on our issue tracker](https://github.com/auth0/auth0-dotnet-templates/issues).

### Vulnerability Reporting

Please do not report security vulnerabilities on the public GitHub issue tracker. The [Responsible Disclosure Program](https://auth0.com/responsible-disclosure-policy) details the procedure for disclosing security issues.

---

<p align="center">
  <picture>
    <source media="(prefers-color-scheme: light)" srcset="https://cdn.auth0.com/website/sdks/logos/auth0_light_mode.png"   width="150">
    <source media="(prefers-color-scheme: dark)" srcset="https://cdn.auth0.com/website/sdks/logos/auth0_dark_mode.png" width="150">
    <img alt="Auth0 Logo" src="https://cdn.auth0.com/website/sdks/logos/auth0_light_mode.png" width="150">
  </picture>
</p>
<p align="center">Auth0 is an easy to implement, adaptable authentication and authorization platform.<br> To learn more check out <a href="https://auth0.com/why-auth0">Why Auth0?</a></p>

