---
name: verify-template-wiring
description: Checks that a template version is fully wired into this repo (csproj lists, template.json, tests, docs, placeholders) and then packs and tests it. Use as the last step after creating a template or adding a .NET version, or any time the user asks to validate a template.
---

# Verify template wiring

## Procedure

1. Run the checker for each template version you touched, from the repo root:

   ```bash
   dotnet run --file .agents/skills/verify-template-wiring/scripts/verify-wiring.cs -- Auth0.Mvc version10
   ```
   It needs the .NET 10 SDK and works on Windows, macOS and Linux. It prints one `PASS:` or `FAIL:` line per check and exits non-zero if any fail. Append `--help` after `--` for details.
2. Fix each FAIL (the message names the missing file or entry), then re-run. Repeat until there are no FAIL lines.
3. Build and test:

   ```bash
   dotnet pack -o ./output
   cd tests && dotnet test
   dotnet test --filter "FullyQualifiedName~TemplateInstantiationTests"
   ```
   If a test fails, read the failure, fix the cause (usually a missing InlineData row or placeholder), and re-run only the failing filter, then the full suite once.
4. Report to the user: which checks passed, what was skipped (for example screenshots), and any pre-existing failures you did not cause.

## What the checker covers
- `version<N>` folder exists and has `register-with-auth0.cmd`.
- Every `registration/` folder has a valid `config.json` with `AppName`, `AppType`, `RegistrationScriptFile`.
- Every `registration/` folder appears in `Auth0Templates.csproj` in both `RegistrationFolders` and a `Copy` line.
- `template.json` has the `net<N>.0` framework choice and a `./version<N>/` source (templates with `.template.config` only).
- `TemplateStructureTests.cs` mentions the version folder.
- A docs page exists for the template's `shortName`, `README.md` links it, and the page's `--framework` values equal the `Framework` choices in `template.json`.
- The ebook image and an `a0.to/dotnet-templates/<slug>` link exist (slug checked for known project types; use `--no-banner` for Console or when the user declined a banner).
- No hardcoded `*.auth0.com` domain in the version folder.

It does not build or run tests; step 3 does.
