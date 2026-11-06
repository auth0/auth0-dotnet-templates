---
name: update-template-docs
description: Creates or updates the documentation for a template (docs/<shortName>.md, README.md, readme-pkg.md, AGENTS.md version statements) so it matches the implementation. Use after a template or .NET version is added or changed, or when the user reports docs that are out of date. AGENTS.md requires docs to stay in sync with the code.
---

# Update template documentation

Docs are part of the change, not a follow-up. Do this after **configure-auth0-registration** so that options, URLs and app types are final.

Inputs: template folder, `shortName`, versions now supported (from `.template.config/template.json`), and whether the page is new or existing.

Checklist:

```
- [ ] 1. Gather the facts from the implementation
- [ ] 2. Create or update docs/<shortName>.md
- [ ] 3. Screenshots
- [ ] 4. README.md and readme-pkg.md
- [ ] 5. Other version statements
- [ ] 6. Run the docs check
```

1. **Facts.** Read from the code, never from memory or from the old page:
   - `Framework` choices and default in `template.json`.
   - Template options in `dotnetcli.host.json` (`--domain`, `--client-id`, `--framework`, plus any template-specific ones such as the Web API `--minimal`/`--controllers`/`--audience`).
   - `AppType`, `GrantTypes` and URLs in `registration/config.json` (they decide the wording "Regular Web", "Native", "SPA" and any note about enabling grant types).
   - The `name` and `shortName` in `template.json`, which appear verbatim in the CLI examples and in the Visual Studio/Rider instructions.
2. **Page.**
   - New template: copy `docs/auth0console.md` (or the closest page by type) to `docs/<shortName>.md` and keep its sections: intro with a link to the relevant Auth0 docs, CLI usage, automatic registration, manual registration with the options list, Visual Studio, Rider, manual post-action. Replace every console-specific sentence.
   - Existing template, new version: update the `-f`/`--framework` line to list exactly the `Framework` choices and the default (`The possible values are ... The default value is ...`; a single value uses `The only supported value is ...`). Remove versions the template no longer offers (for example a stale `net7.0`). Check other sentences that mention a version.
   - Options listed on the page must equal the options in `dotnetcli.host.json`.
3. **Screenshots.** Pages reference `assets/auth0-<name>-app-vs.png`, `-vs-options.png`, `-rider.png`, and for some templates `-vs-mac.png` and `-vs-mac-options.png`. You cannot take them. Keep the references, list every image that does not exist in `docs/assets/` in your final report, and do not invent files.
4. **README.** In `README.md` keep one bullet per template in the usage list: `- [Auth0 ... Application](docs/<shortName>.md)`, with the same display name as `template.json`. Mirror any change in `readme-pkg.md` (the NuGet package readme).
5. **Other statements.** Search for version claims and update them:

   ```bash
   grep -rn -i "net[0-9]*\.0\|\.NET [0-9]" README.md readme-pkg.md AGENTS.md CONTRIBUTING.md docs/*.md cli-wrapper/README.md
   ```
   Typical places: the "Templates support .NET 8, 9, and 10" sentence in `AGENTS.md`, supported-version sentences in the READMEs, and `cli-wrapper/README.md` when the cli-wrapper changed.
6. **Check.** Run **verify-template-wiring**; its docs checks compare the page with `template.json`. Fix every FAIL.

## Gotchas
- Visual Studio and Rider do not run post actions, so the manual registration section must stay (`./register-with-auth0.cmd`).
- The Auth0 CLI automatic-registration example output in the pages is copied text; update only the template name and path, not the structure.
- Do not document options the template does not have. In particular, check `dotnetcli.host.json` for the Web API template.
- Existing stale text that is unrelated to your change (other templates' pages) should be reported, not fixed, unless the user asks.
