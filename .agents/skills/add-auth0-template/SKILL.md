---
name: add-auth0-template
description: Creates a brand-new Auth0 project template (a new project type) for a specific .NET version, adding Auth0 authentication or authorization and wiring it into the repo. Use when the user asks for a new template such as "add an Auth0 worker service template for .NET 10" or "create a gRPC template".
---

# Add a new Auth0 template

Entry point for the "new project type" workflow. For adding an existing template to a newer .NET version, use **add-template-dotnet-version** instead.

## Inputs (ask only for what is missing)
- Project type (for example Worker Service, gRPC, Razor Pages).
- .NET version `N`.
- Auth0 scenario: **authentication** (users sign in; needs a client id) or **authorization** (the app validates access tokens; needs an audience). Default: pick from the table in `references/template-catalog.md` for similar types; ask if unclear.

## Procedure

Copy this checklist and tick as you go:

```
- [ ] 1. Decide names and scenario
- [ ] 2. Scaffold the base project
- [ ] 3. Integrate Auth0
- [ ] 4. Add the ebook banner
- [ ] 5. Convert to a template
- [ ] 6. Configure registration
- [ ] 7. Wire into the repo
- [ ] 8. Update docs
- [ ] 9. Verify
```

1. **Names.** Read `references/template-catalog.md`. Pick the closest existing template as the structural model and derive: folder `templates/Auth0.<Name>`, `sourceName` `Auth0<Name>`, `shortName` `auth0<lowercase>`, `AppType` (`regular` web app, `native` for apps with no client secret, `api` for token validation). State these to the user in one line before continuing.
2. **Scaffold.** Run the **scaffold-dotnet-project** skill. Stop if the SDK is missing.
3. **Integrate.** Run the **integrate-auth0** skill. If the project type has no matching reference file, follow its "New project type" section.
4. **Assets.** Run the **add-auth0-assets** skill. For a new project type it asks the user for the a0.to slug; for Console there is no banner.
5. **Convert.** Run the **convert-to-template** skill.
6. **Registration.** Run the **configure-auth0-registration** skill.
7. **Wire.** Run the **wire-template-into-repo** skill.
8. **Docs.** Run the **update-template-docs** skill.
9. **Verify.** Run the **verify-template-wiring** skill and loop until every check passes.

## Rules
- Never put real credentials in the template; use the placeholders from `AGENTS.md`.
- Never reuse a `shortName` or `identity` from another template.

## Gotchas
- Console's `template.json` lists only `net10.0` in `Framework`; a template created for version `N` lists only the versions that actually exist.
