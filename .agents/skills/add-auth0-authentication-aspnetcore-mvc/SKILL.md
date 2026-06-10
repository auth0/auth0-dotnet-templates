---
name: add-auth0-authentication-aspnetcore-mvc
description: Add Auth0 authentication to an ASP.NET Core MVC web application, including login, logout, and user profile.
---

# Auth0 ASP.NET Core Web App Integration

Add login, logout, and user profile to an ASP.NET Core MVC application using `Auth0.AspNetCore.Authentication`.

---

## Prerequisites

- ASP.NET Core MVC application (.NET 8 or higher)

## When NOT to Use

- **ASP.NET Core Web APIs** 
- **Blazor Web applications**
- **Single Page Applications using Blazor WebAssembly**
- **Blazor Hybrid applications**
- **MAUI applications**
- **Console applications**

---

## Integration Workflow

### 1. Install SDK

```bash
dotnet add package Auth0.AspNetCore.Authentication
```

### 2. Configure Credentials

Add Auth0 settings to `appsettings.json`:

```json
{
  "Auth0": {
    "Domain": "{DOMAIN}",
    "ClientId": "{CLIENT_ID}"
  }
}
```


### 3. Register Auth0 in Program.cs

Add the code highlighted below:

```csharp
using Auth0.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// 👇 new code
builder.Services.AddAuth0WebAppAuthentication(options =>
{
      options.Domain = builder.Configuration["Auth0:Domain"];
      options.ClientId = builder.Configuration["Auth0:ClientId"];
      options.Scope = "openid profile email";
});
// 👆 new code

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Standard middleware...
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();    // 👈 new code
app.UseAuthorization();     // 👈 new code

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

**Critical:** `UseAuthentication()` must come before `UseAuthorization()`. Reversing these causes silent auth failures where protected routes are never challenged.

### 4. Create AccountController

Create a new file `Controllers/AccountController.cs` with the following content:

```csharp
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    public async Task Login(string returnUrl = "/")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    [Authorize]
    public async Task Logout()
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            .WithRedirectUri(Url.Action("Index", "Home"))
            .Build();

        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

  [Authorize]
  public IActionResult Profile()
  {
    return View(new UserProfileViewModel()
    {
      Name = User.Identity?.Name,
      EmailAddress = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value,
      ProfileImage = User.FindFirst(c => c.Type == "picture")?.Value
    });
  }
}
```

`Login` does not need `[Authorize]` - it is the entry point for unauthenticated users.
`Logout` requires `[Authorize]` to ensure the sign-out only fires for authenticated sessions.
**Always call both `SignOutAsync` methods** - signing out of only the Auth0 scheme leaves a local cookie; signing out of only the cookie scheme skips the Auth0 logout URL.

### 5. Create Profile View

Create a new file `Models/UserProfileViewModel.cs`:

```csharp
namespace Auth0Mvc.ViewModels;

public class UserProfileViewModel
{
  public string? EmailAddress { get; set; }

  public string? Name { get; set; }

  public string? ProfileImage { get; set; }
}
```

Create `Views/Account/Profile.cshtml`:

```html
@model Auth0Mvc.ViewModels.UserProfileViewModel

@{
    ViewData["Title"] = "User Profile";
}

<div class="row">
    <div class="col-md-12">
        <div class="row">
            <h2>@ViewData["Title"].</h2>

            <div class="col-md-2">
                <img src="@Model.ProfileImage"
                     alt="" class="img-rounded img-responsive" />
            </div>
            <div class="col-md-4">
                <h3>@Model.Name</h3>
                <p>
                    <i class="glyphicon glyphicon-envelope"></i> @Model.EmailAddress
                </p>
            </div>
        </div>
    </div>
</div>
```

### 7. Update Navigation (_Layout.cshtml)

Add login/logout/profile links to your nav bar inside `Shared/_Layout.cshtml`. Add the code highlighted below:

```html
<!-- ...existing code... -->

<div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
<!-- 👇 new code -->
@if (User.Identity.IsAuthenticated)
{
<!-- 👆 new code -->
    <ul class="navbar-nav flex-grow-1">
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Index">Home</a>
        </li>
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
        </li>
    </ul>
    <!-- 👇 new code -->
    <ul class="navbar-nav ms-auto">
        <li class="nav-item">
            <a class="nav-link text-dark" 
                asp-controller="Account" 
                asp-action="Profile">Hello @User.Identity.Name!</a>
        </li>
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" 
                asp-controller="Account" 
                asp-action="Logout">Logout</a>
        </li>
    </ul>
}
else
{
    <li class="nav-item">
        <a class="nav-link text-dark" asp-controller="Account" asp-action="Login">Login</a>
    </li>
}
<!-- 👆 new code -->
</div>

<!-- ...existing code... -->
```

### 7. Standardize TCP port

- Make sure your app is configured to run on port 5000 for HTTP and 5001 for HTTPS by modifying the `applicationUrl` property in `Properties/launchSettings.json`.

- Make sure that the https profile in `launchSettings.json` is the first profile in the list, as this is the one that `dotnet run` will use by default:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "https": {
        //...existing settings...
    },
    "http": {
        //...existing settings...
    }
  }
}
```

### 8. Test the App

```bash
dotnet run
```

Visit `http://localhost:5000` and click Login to start the Auth0 login flow.

