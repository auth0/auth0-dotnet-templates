---
name: add-auth0-assets
description: Add Auth0 marketing assets to the project.
---

# Add Auth0 .NET ebook banner

Add the Auth0 .NET ebook banner to the project's home page based on the project template type.

## ASP.NET Core MVC Project

- Copy the `auth0-dotnet-ebook.png` image from the `assets/images` folder to the `wwwroot/images` folder in your project.

- Replace the content of the home page in `Views/Home/Index.cshtml` with the following code:

```html
<!-- ...existing code... -->

<div class="text-center">
    <h1 class="display-4">Welcome</h1>
        <div>
        <a href="https://a0.to/dotnet-templates/mvc"><img src="images/auth0-dotnet-ebook.png" alt=".NET Identity with Auth0"></a>
    </div>
</div>

<!-- ...existing code... -->
```