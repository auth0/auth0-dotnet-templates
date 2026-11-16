---
name: scaffold-dotnet-project
description: Creates a plain, vanilla .NET project for a given framework version in a scratch folder using `dotnet new`, as the starting point for an Auth0 template. Use before integrating Auth0 whenever a template for a new .NET version or project type is being built.
---

# Scaffold the base project

Produces a clean project on the real SDK for the target version. This is the source of truth for the framework's default code, so never copy it from an older `versionN` folder.

## Procedure

1. Check the SDK: `dotnet --list-sdks`. A `N.x` entry must exist. If not, **stop** and ask the user to install it; do not retarget with `-f` on another SDK.
2. Pick the command from `.agents/skills/add-auth0-template/references/template-catalog.md` (column "Base project"). Check the exact options for this SDK first with `dotnet new <type> --help`, because option names change between releases.
3. Create a scratch folder outside the repo and scaffold into it, naming the project after the template's `sourceName`:

   ```bash
   scratch="$(mktemp -d)"
   dotnet new <type> -n <SourceName> -o "$scratch/<SourceName>" -f net<N>.0 <extra options>
   ```
   Examples: `dotnet new mvc -n Auth0Mvc -o "$scratch/Auth0Mvc" -f net11.0`, `dotnet new webapi -n Auth0MinimalWebAPI ...` (add `--use-controllers` for the controller variant).
4. Confirm it builds before touching it: `dotnet build "$scratch/<SourceName>"`. If it fails on a clean scaffold, report the error; do not continue.
5. Hand the scratch path to the next skill (**integrate-auth0**).

## Gotchas
- `-f` only accepts frameworks the installed SDK supports. A preview SDK may need `--allow-scripts no` to avoid prompts; always pass flags so nothing asks interactively.
- Remove `bin/`, `obj/` and `.git` from the scratch output before it is copied into the repo.
- Project name is the `sourceName` (for example `Auth0Mvc`), because the template engine replaces that token with the user's project name. Do not use any other name or the replacement will miss.
- Generated `launchSettings.json` ports are random; they are normalised later by **convert-to-template**.
