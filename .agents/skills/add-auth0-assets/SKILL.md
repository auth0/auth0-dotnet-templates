---
name: add-auth0-assets
description: Adds the Auth0 .NET ebook banner (image plus tracked a0.to link) to a template's sample UI, with the link slug and placement that match the project type. Use after integrate-auth0 when creating a new template or a new .NET version, because a freshly scaffolded project has no banner.
---

# Add the Auth0 ebook banner

Every template except the console template shows the `.NET Identity with Auth0` ebook image, linked to `https://a0.to/dotnet-templates/<slug>`. The slug identifies the project type in Auth0's campaign tracking, so it must be right.

Input: the scratch project (from **integrate-auth0**) and the template folder name.

Checklist:

```
- [ ] 1. Decide the slug
- [ ] 2. Copy the image
- [ ] 3. Insert the banner and link
- [ ] 4. Build
```

1. **Slug.** Look up the project type in [references/placement.md](references/placement.md).
   - Known type: use its slug.
   - Console: **no banner**. Skip this skill entirely.
   - New project type (not in the table): **ask the user for the slug**, and for whether a banner is wanted at all. Never invent a short link. If the user says no banner, record that in your report and later pass `--no-banner` to the verifier.
2. **Image.** Copy the existing PNG from `assets/images/auth0-dotnet-ebook.png`; never recreate or edit it. Destination folder and file name per type are in the table (MAUI uses underscores; the BFF client uses `src/assets/`).
3. **Insert.** Add the link and image at the location in the table, using the snippet for that framework from `references/placement.md`. Keep the same alt text, `.NET Identity with Auth0`. Match the surrounding markup (indentation, `text-center` wrapper) of the page you edit.
4. **Build.** `dotnet build` the scratch project (or `npm run build` for the BFF client). For MAUI the image must be under `Resources/Images/`, which the project already includes as a `MauiImage`.

## Gotchas
- A project that has several UIs gets one banner per UI: for example the Web API and the BFF's `WeatherForecastAPI` both use slug `webapi`, while the BFF client uses `bff`.
- The Web API also puts the same slug in the Swagger contact URL (`SwaggerExtensions.cs`, `Contact.Url`); update both places together.
- Blazor WebAssembly shows the banner twice: the home page and the `NotAuthorized` block in `App.razor`.
- Image paths are relative: `images/auth0-dotnet-ebook.png` for web projects served from `wwwroot`, and `src/assets/auth0-dotnet-ebook.png` for the BFF's Vite client.
- The image is a binary asset copied into every template version; it is not generated at pack time.
