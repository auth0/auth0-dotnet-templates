# Template catalog

Read this to map a template to its base `dotnet new` command, Auth0 package and registration settings. All paths are under `templates/`. Verify package versions on NuGet before using them (see `integrate-auth0/references/nuget-versions.md`); the versions below are what the repo used at the time of writing.

| Folder | shortName | Base project (`dotnet new ... -f netN.0`) | Auth0 package | AppType | Placeholders live in |
|---|---|---|---|---|---|
| Auth0.Mvc | auth0webapp | `mvc` | Auth0.AspNetCore.Authentication 1.7.0 | regular | `appsettings.json` |
| Auth0.WebAPI (controllers) | auth0webapi | `webapi --use-controllers` | Auth0.AspNetCore.Authentication.Api | api | `appsettings.json` (`Auth0:Domain` = `{DOMAIN}`, `Auth0:Audience` = `{AUDIENCE}`) |
| Auth0.MinimalWebAPI | (source for auth0webapi, default) | `webapi` | Auth0.AspNetCore.Authentication.Api | api | `appsettings.json` |
| Auth0.MCPServer | auth0mcpserver | `mcpserver -t remote` (needs `Microsoft.McpServer.ProjectTemplates`) | Auth0.AspNetCore.Authentication.Api + ModelContextProtocol.AspNetCore | api | `appsettings.json` (`Auth0:Domain`, `Auth0:Audience`) |
| Auth0.BlazorWebApp | auth0blazor | `blazor --interactivity Auto` (server + `.Client`) | Auth0.AspNetCore.Authentication 1.7.0 | regular | `appsettings.json` |
| Auth0.BlazorWebAssembly | auth0blazorwasm | `blazorwasm` (.NET 8/9 hosted layout: Client/Server/Shared; .NET 10: single project) | `Microsoft.AspNetCore.Components.WebAssembly.Authentication` (OIDC) | spa | `wwwroot/appsettings.json` (`Auth0:Authority` = `https://{DOMAIN}`, `Auth0:ClientId`) |
| Auth0.Maui | auth0maui | `maui` | Auth0.OidcClient.MAUI 1.4.0 | native | `MauiProgram.cs` |
| Auth0.BackendForFrontend | auth0bff | solution: server + API + client | Auth0.AspNetCore.Authentication 1.7.0 | regular (+ api for the API project) | `appsettings.json` |
| Auth0.Console | auth0console | `console` | Auth0.AuthenticationApi 7.46.0 | native (`GrantTypes`: `code,implicit,refresh-token,device-code`) | `Program.cs` |
| Auth0.BlazorServer | auth0blazorserver | discontinued, no `versionN/` folder; do not extend | | | |

## Registration
Callbacks, logout URLs, `AppType` and multi-project layout: see `.agents/skills/configure-auth0-registration/references/registration-values.md`.

## Choosing a model for a new type
- Server-rendered web app with sign-in: copy the structure of Auth0.Mvc.
- Service that validates bearer tokens: Auth0.MinimalWebAPI.
- Headless or device app: Auth0.Console.
- Native/mobile/desktop: Auth0.Maui.
