# Choosing NuGet package versions

Never guess versions and never copy them from an older `versionN` folder.

1. Microsoft packages (`Microsoft.AspNetCore.*`, `Microsoft.Maui.Controls`, `Microsoft.Extensions.*`): use the version the scaffolded project already references, or the latest stable `N.x.y` for the target framework.
2. Auth0 packages (`Auth0.AspNetCore.Authentication`, `Auth0.AspNetCore.Authentication.Api`, `Auth0.OidcClient.MAUI`, `Auth0.AuthenticationApi`): use the latest stable version that supports the target framework.
3. Add packages without `--version`; the SDK then resolves the latest stable release for the project's framework:

   ```bash
   dotnet add <project> package <PackageId>
   ```
   Read the resolved version back from the `.csproj`. For Microsoft packages that must match the framework, pass `--version N.*` (for example `--version 11.*`) so a newer major is not picked.
4. To only look versions up, `dotnet package search <PackageId> --exact-match --format json` lists **every** published version, oldest first, and may include prereleases. Take the highest version without a `-` suffix. Do not take the first entry.
5. If the latest Auth0 package does not support the new framework (restore error NU1202), stop and report it; do not downgrade the framework.
6. Record the final package versions in your report so the docs page can mention them.
