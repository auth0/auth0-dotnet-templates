# Registration values by template type

Models: `templates/Auth0.<T>/version10/registration/config.json`.

| Type | `AppType` | `Callbacks` | `LogoutUrls` | Other |
|---|---|---|---|---|
| Web app (Mvc, Blazor Web App) | `regular` | `https://localhost:5001/callback` | `https://localhost:5001/` | |
| Blazor WebAssembly | `spa` | `https://localhost:5001/authentication/login-callback` | `https://localhost:5001` | |
| BFF server | `regular` | `https://localhost:54575/callback` | `https://localhost:54575/` | port differs from other web apps |
| API (Web API, minimal API, BFF API project) | `api` | `""` | `""` | needs `{AUDIENCE}` |
| MAUI | `native` | `myapp://callback/` | `myapp://callback/` | |
| Console (device flow) | `native` | `""` | `""` | `"GrantTypes": "code,implicit,refresh-token,device-code"` |

## Multi-project templates
Blazor Web App, Blazor WebAssembly (.NET 8/9) and BFF have several projects. Each project to register gets its own `registration/config.json`. One `register-with-auth0.cmd` at the version root calls each project's `registration/cli-wrapper.dll` (see `Auth0.BackendForFrontend/version10`). Registration folders found today:

- `Auth0.BlazorWebApp/version<N>/Auth0BlazorWebApp/registration`
- `Auth0.BlazorWebAssembly/version8|9/Client|Server/registration`, `version10/registration`
- `Auth0.BackendForFrontend/version10/Auth0BackendForFrontend.Server/registration` and `.../WeatherForecastAPI/registration`

## Placeholders
`{DOMAIN}`, `{CLIENT_ID}`, `{CLIENT_SECRET}`, `{AUDIENCE}` only. Never a real tenant value.
