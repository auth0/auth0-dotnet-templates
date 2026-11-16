---
name: configure-auth0-registration
description: Sets up automatic Auth0 application registration for a template version: register-with-auth0.cmd, registration/config.json, the template.json autoregister wiring, and the cli-wrapper entries in Auth0Templates.csproj. Use after convert-to-template, whenever a template or template version is created, or when registration settings (callbacks, app type, grant types) need to change.
---

# Configure Auth0 registration

At `dotnet new` time the template runs `register-with-auth0.cmd`, which runs the bundled `cli-wrapper`, which calls the Auth0 CLI to create the application and writes the tenant values into the files listed in `AppSettingsFiles`. This skill creates every piece of that chain for one template version.

Input: `templates/Auth0.<Name>/version<N>/` (from **convert-to-template**), the Auth0 scenario, and the HTTPS port used in `launchSettings.json`.

Values for `AppType`, `Callbacks`, `LogoutUrls` and `GrantTypes` are in [references/registration-values.md](references/registration-values.md). Read it before step 2.

Checklist:

```
- [ ] 1. register-with-auth0.cmd
- [ ] 2. registration/config.json for each project
- [ ] 3. template.json autoregister wiring
- [ ] 4. Auth0Templates.csproj (list + Copy)
- [ ] 5. cli-wrapper changes (only if needed)
```

1. **Script.** Put `register-with-auth0.cmd` at the root of `version<N>`. Copy it from `templates/Auth0.Mvc/version10/register-with-auth0.cmd` (single project) or `templates/Auth0.BackendForFrontend/version10/register-with-auth0.cmd` (several projects: one `dotnet <project>/registration/cli-wrapper.dll` line per project, in both the sh and cmd halves). Keep the `rem #if (OS != "Windows_NT")` structure byte for byte: it is one file that is both a shell script and a batch file.
2. **Config.** Create `registration/config.json` beside every project that must be registered, with:
   - `AppName`: the template `sourceName`; `AppDescription`: one line.
   - `AppType`, `Callbacks`, `LogoutUrls`, optional `GrantTypes`: from the values reference. Callback ports must equal the HTTPS port in `Properties/launchSettings.json`.
   - `AppSettingsFiles`: every file that contains `{DOMAIN}`, `{CLIENT_ID}`, `{AUDIENCE}`, relative to the folder holding `registration/`.
   - `RegistrationScriptFile`: relative path to the script (`../register-with-auth0.cmd`, or `../../` for a project one level below the version root).
   - `Verbose`: false.
   Validate that each file parses as JSON.
3. **`template.json`.** Confirm (copying from the model if missing): the `autoregister` computed symbol, both `postActions` (`chmod +x` on non-Windows, and the `register-with-auth0.cmd` action conditioned on `autoregister`), and, in each `sources` entry, the `(!autoregister)` modifier excluding `registration/**/*`. Leave them unchanged otherwise. For API templates the `autoregister` condition compares `domain` and `audience` instead of `clientId`.
4. **`Auth0Templates.csproj`.** For every `registration` folder add the path to `<RegistrationFolders Include=...>` **and** a `<Copy SourceFiles="@(CliWrapperFiles)" DestinationFolder="...\registration" SkipUnchangedFiles="false" />` line in the same order, using backslashes. A folder missing from either place ships without `cli-wrapper.dll`. Do not copy any `cli-wrapper.*` file by hand: they are generated at pack time and gitignored.
5. **`cli-wrapper/`.** Change it only if the template needs behaviour it lacks (a new `AppType`, a new config key such as `GrantTypes`, a new app-settings format). State why, change `CliWrapper.cs` / `ConfigData.cs`, add tests in `tests/Unit/CliWrapperTests.cs`, and document the option in `cli-wrapper/README.md`.

Then run **verify-template-wiring**, which checks steps 1, 2 and 4.

## Gotchas
- The BFF server uses port `54575` and the API project has no callbacks; do not apply the 5001 default blindly.
- Templates without an `appsettings.json` (Console, MAUI) have the placeholders in source files; list those source files in `AppSettingsFiles`.
- In multi-project templates, `AppSettingsFiles` in the BFF configs are written relative to the version root, not to the project; follow the model template for the same shape.
- Do not delete `register-with-auth0.cmd` logic that removes the script on Windows (`start /b "" cmd /c del "%~f0"`): it is intentional.
