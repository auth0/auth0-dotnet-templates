---
name: convert-to-template
description: Turns a scaffolded, Auth0-integrated .NET project into a `dotnet new` template in this repo (version folder, template.json, host files, ports). Use after integrate-auth0, when creating a new template or adding a version to an existing one.
---

# Convert the project into a template

Input: the scratch project (builds, has Auth0 placeholders), the template folder name `Auth0.<Name>`, version `N`, and the `sourceName`.

The live files in `templates/` are the models. Copy the closest one and edit it; do not write these files from memory.

| Need | Model file |
|---|---|
| Web template with ports | `templates/Auth0.Mvc/.template.config/template.json` |
| Non-web template (no ports) | `templates/Auth0.Console/.template.config/template.json` |
| API template with `{AUDIENCE}` and OpenAPI options | `templates/Auth0.WebAPI/.template.config/template.json` |

## Procedure

Checklist:

```
- [ ] 1. Copy the project into templates/Auth0.<Name>/version<N>/
- [ ] 2. Normalise ports in launchSettings.json
- [ ] 3. Create or update .template.config
- [ ] 4. Smoke-test locally
- [ ] 5. Run configure-auth0-registration
```

1. **Copy.** `rsync -a --exclude bin --exclude obj --exclude .git "$scratch/<SourceName>/" templates/Auth0.<Name>/version<N>/`. Keep the `sourceName` token in file names, namespaces and project names. For a multi-project template, keep the same project layout as the template's newest existing version.
2. **Ports.** Web templates use these literals in `Properties/launchSettings.json` so the template engine can replace them: Kestrel HTTP `5000`, Kestrel HTTPS `5001`, IIS HTTP `8080`, IIS HTTPS `44300` (see the `*PortReplacer` symbols in the Mvc `template.json`). Replace the random ports from the scaffold with these. BFF is the one exception (its server uses `54575`); copy that template's convention if the new template resembles it.
3. **`.template.config`.**
   - New template: create `template.json`, `dotnetcli.host.json`, `ide.host.json` from the models, and copy `auth0.png` from another template's `.template.config`. Set `groupIdentity` `Auth0.NET.<Name>`, `identity` `Auth0.NET.<Name>.CSharp`, `shortName`, `sourceName`, `defaultName`, `name`, `description`, `classifications`. Keep the `guids` entry identical to the others (it is shared by design).
   - New version of an existing template: in `template.json`, add the `netN.0` entry at the top of `Framework.choices`, add a `sources` entry with condition `Framework == "netN.0"`, source `./version<N>/` and the `(!autoregister)` modifier that excludes `registration/**/*`. Change the `Framework` default only if the user said so. For Web API, also add entries for the minimal variant (`../Auth0.MinimalWebAPI/version<N>/`).
   - Keep the symbols `domain`, `clientId` (or `audience`), `Framework`, `autoregister`, and both `postActions` unchanged (the registration wiring is verified by **configure-auth0-registration**).
   - Update the "supported value" wording in docs later (**wire-template-into-repo**).
4. **Smoke test.** Check that no file in `version<N>` contains a real tenant domain or client id. Do not JSON-parse `template.json`: the existing ones contain `//` comments. The full install-and-instantiate check happens in **verify-template-wiring**.
5. **Registration.** Hand over to **configure-auth0-registration** for `register-with-auth0.cmd`, `registration/config.json` and the csproj entries.

## Gotchas
- Do not copy `registration/cli-wrapper.*` binaries into the version folder; they are generated at pack time and gitignored.
- `Auth0.MinimalWebAPI` has no `.template.config`; its sources are consumed by `Auth0.WebAPI/.template.config/template.json`.
- `Auth0.Mvc/version9/Auth0Mvc.csproj` has a malformed `PackageReference` (missing quote). Do not use it as a model; report it to the user.
- A trailing `description ` key (with a space) exists in the second postAction of existing templates. It is harmless; copy the file rather than retyping it.
