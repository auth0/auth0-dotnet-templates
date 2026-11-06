---
name: wire-template-into-repo
description: Updates what a new template or template version requires outside the template folder: structure and instantiation tests and the package version. Use after configure-auth0-registration.
---

# Wire the template into the repo

Reference commit for a whole new template: `git show 82ca40d --stat` ("Add Console template"). Read that diff if unsure about any step.

Checklist:

```
- [ ] 1. TemplateStructureTests.cs
- [ ] 2. TemplateInstantiationTests.cs
- [ ] 3. Package version (only if asked)
```

1. **`tests/TemplateValidation/TemplateStructureTests.cs`.** Add `[InlineData(...)]` rows for the new template or version to each theory that lists templates: the template-config theory (template root), the registration-config theories (path of the folder containing `registration/`), the app-type and callbacks theories that match the template's `AppType`, and `GetAllTemplatePaths()`. Search the file for a sibling version of the same template (`grep -n "Auth0.Mvc/version10"`) and add a row next to every hit. Follow the naming convention `Method_Does_WhenCondition`, and FluentAssertions for any new test.
2. **`tests/TemplateValidation/TemplateInstantiationTests.cs`.** For a new template, add a theory modelled on `ConsoleTemplate_Instantiates_AndBuilds`. For a new version, add the `(shortName, "netN.0")` row to the existing theory. These tests need `dotnet pack` first.
3. **Package version.** Bump `<PackageVersion>` in `Auth0Templates.csproj` only when the user asks.

The `Auth0Templates.csproj` registration entries and any `cli-wrapper/` change belong to **configure-auth0-registration**. Documentation (`docs/`, READMEs) belongs to **update-template-docs**.

## Gotchas
- Test rows use `Auth0.X/version10` (the folder that contains `registration`); the csproj form is covered by **configure-auth0-registration**.
- Existing gaps (for example `Auth0.MinimalWebAPI` missing from the config-file theory) are not yours to fix unless asked; mention them.
- Do not hand-edit generated files under `output/`.
