---
name: integrate-auth0
description: Adds Auth0 authentication or authorization to a freshly scaffolded .NET project (packages, Program.cs, configuration with placeholders) following this repo's existing template patterns. Use after scaffolding a base project, for MVC, Web API, Blazor, MAUI, console, or BFF templates.
---

# Integrate Auth0 into a scaffolded project

Input: the scratch project from **scaffold-dotnet-project** and the project type.

## Procedure

1. Open the reference for the project type and follow it. Read only the one you need:
   - MVC and Blazor Web App server: [references/web-app.md](references/web-app.md)
   - Web API (minimal or controllers): [references/web-api.md](references/web-api.md)
   - Blazor WebAssembly: [references/blazor-wasm.md](references/blazor-wasm.md)
   - MAUI and console: [references/native-and-console.md](references/native-and-console.md)
   - BFF (server + API + client): combine web-app and web-api; model on `templates/Auth0.BackendForFrontend/version10`
   - New project type: see "New project type" below
2. Choose package versions as described in [references/nuget-versions.md](references/nuget-versions.md). Do not copy versions from older folders.
3. Use only the placeholders `{DOMAIN}`, `{CLIENT_ID}`, `{CLIENT_SECRET}`, `{AUDIENCE}` for tenant values. Never write a real domain, client id or secret.
4. Keep the sample app small: sign-in/sign-out (or a protected endpoint) plus a visible way to see the result. Remove unrelated scaffold sample code only if the existing template of the same type did.
5. Validate: `dotnet build <project>` must succeed with zero warnings introduced by your changes. If it fails, fix and rebuild; do not continue to the next skill with a failing build.

## New project type
1. Find the closest existing template in `templates/` and read its newest `Program.cs`, `.csproj` and settings file.
2. Pick the Auth0 mechanism: sign-in via `Auth0.AspNetCore.Authentication` (server apps), `Auth0.OidcClient.*` (native apps), `Auth0.AuthenticationApi` (device flow, headless), or JWT bearer validation (APIs).
3. Check the current Auth0 docs for that SDK for the minimal configuration; link to them from the docs page later.
4. Put placeholders where the closest existing template puts them (settings file if the type has one, otherwise source).

## Gotchas
- Server apps read `Auth0:Domain` and `Auth0:ClientId` from `appsettings.json`; APIs use the `Authentication:Schemes:Bearer` section; Blazor WebAssembly binds the `Auth0` section with `Authority`.
- The Web API `ValidIssuer` in existing templates is `{DOMAIN}`; keep the same shape unless the framework requires a trailing slash.
- Web API templates use `#if (!removeOpenAPI)` template conditionals around OpenAPI/Swagger code; preserve them when copying patterns.
