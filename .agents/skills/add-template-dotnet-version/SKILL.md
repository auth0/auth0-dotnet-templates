---
name: add-template-dotnet-version
description: Adds support for a new .NET version (for example net11.0) to every existing Auth0 project template in this repo, by scaffolding each project with `dotnet new` on that SDK and re-integrating Auth0. Use when the user asks to "support .NET N", "add version N", or "update the templates for the new .NET release".
---

# Add a new .NET version to all templates

Entry point for the "new .NET version" workflow. It orchestrates the building-block skills once per template. It does not copy the previous `versionN` folder: each project is scaffolded fresh with `dotnet new` for the target SDK, then Auth0 is added.

## Inputs
- `N`: the major .NET version (for example `11`). If not given, ask for it. Never guess.

## Procedure

1. Preflight. Run `dotnet run --file .agents/skills/add-template-dotnet-version/scripts/list-templates.cs` from the repo root (needs the .NET 10 SDK; works on Windows, macOS and Linux). It prints a JSON array with one object per folder in `templates/`: `name`, `shortName`, `versions`, `hasTemplateConfig`, `projects`. Then confirm the SDK is present: `dotnet --list-sdks` must show `N.x`. If it does not, stop and tell the user to install it. Do not fall back to another SDK.
2. Build the checklist. Copy this into your reply and keep it updated:

   ```
   - [ ] Auth0.Mvc
   - [ ] Auth0.WebAPI + Auth0.MinimalWebAPI (one template, two source folders)
   - [ ] Auth0.BlazorWebApp
   - [ ] Auth0.BlazorWebAssembly
   - [ ] Auth0.Maui
   - [ ] Auth0.BackendForFrontend
   - [ ] Auth0.Console
   ```
   Use the real list from step 1; skip any template with `versions: []` (such as the discontinued `Auth0.BlazorServer`). Templates that only exist for newer versions (BFF, Console) are still included.
3. For each template, in order:
   1. Read `.agents/skills/add-auth0-template/references/template-catalog.md` to find the base `dotnet new` command, Auth0 package and `AppType` for this template.
   2. Run the **scaffold-dotnet-project** skill with that command and `N`.
   3. Run the **integrate-auth0** skill for this project type.
   4. Run the **add-auth0-assets** skill (skip for Console: no banner).
   5. Run the **convert-to-template** skill to produce `templates/<T>/version<N>/` and update `.template.config/template.json`.
   6. Run the **configure-auth0-registration** skill.
   7. Run the **wire-template-into-repo** skill.
   8. Run the **update-template-docs** skill.
   9. Tick the checklist row.
4. Bump the package version in `Auth0Templates.csproj` only if the user asked for it (a new .NET version is normally a minor bump).
5. Run the **verify-template-wiring** skill for every template touched, and fix every FAIL before reporting.

## Rules
- Work one template at a time and finish it (including its build) before starting the next.
- Template `Framework` choices stay ordered newest first, and the default becomes the new version only if the user confirms it.
- Templates that already contain `version<N>` are skipped and reported, never overwritten.
- Report at the end: templates done, templates skipped and why, anything needing manual follow-up (screenshots, docs text).

## Gotchas
- `Auth0.WebAPI` is served by two source folders: `Auth0.WebAPI/version<N>` (controllers) and `Auth0.MinimalWebAPI/version<N>` (minimal API, the default). Both need a new version, and `Auth0.WebAPI/.template.config/template.json` references both.
- Blazor Web App has two projects (server and `.Client`) and Blazor WebAssembly changed layout between .NET 9 and 10 (three projects, then one). Scaffold with the same `dotnet new` layout the current newest version uses, and check `template-catalog.md`.
- MAUI needs the MAUI workload (`dotnet workload list`). If it is missing, stop and ask the user rather than installing it.
