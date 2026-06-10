---
name: add-auth0-authentication-blazor
description: Add Auth0 authentication to a Blazor web application (.NET 8 or higher), including login, logout, and user profile.
---

# Auth0 Blazor App Integration

Add login, logout, and user profile to a Blazor web application using `Auth0.AspNetCore.Authentication`.

---

## Prerequisites

- Blazor web application (.NET 8 or higher)

## When NOT to Use

- **ASP.NET Core Web APIs** 
- **ASP.NET Core MVC applications**
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

### 3. Register Auth0 in the server-side Blazor app

Add the code highlighted below to `Program.cs` of the server-side Blazor app:

```csharp
using BlazorIntAuto.Client.Pages;
using BlazorIntAuto.Components;
using Auth0.AspNetCore.Authentication;              // 👈 new code
using Microsoft.AspNetCore.Authentication;          // 👈 new code
using Microsoft.AspNetCore.Authentication.Cookies;  // 👈 new code

var builder = WebApplication.CreateBuilder(args);

// 👇 new code
builder.Services
    .AddAuth0WebAppAuthentication(options => {
      options.Domain = builder.Configuration["Auth0:Domain"];
      options.ClientId = builder.Configuration["Auth0:ClientId"];
    });
// 👆 new code

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// ...existing code...

app.Run();
```

### 4. Protect your Blazor App UI

In `Components/Routes.razor` of the server-side Blazor app, replace the `<RouteView>` component with the `<AuthorizeRouteView>` component, which allows your app to display protected components only to authorized users, as in the following example:

```html
<Router AppAssembly="@typeof(Program).Assembly" AdditionalAssemblies="new[] { typeof(Client._Imports).Assembly }">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="@routeData" Selector="h1" />
    </Found>
</Router>
```

In the `Components/Pages/Weather.razor` file, add the `[Authorize]` attribute to the `Weather` component to protect it from unauthenticated users:

```html
@page "/weather"
@attribute [Authorize] <!-- 👈 new code -->
@attribute [StreamRendering]

<PageTitle>Weather</PageTitle>

<h1>Weather</h1>

<!-- ...existing code... -->
```

In the `Components/_Imports.razor` file of the server-side Blazor app, add the lines highlighted below:

```csharp
//...other using statements...
@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
```

### 5. Add Login and Logout endpoints

In `Program.cs` of the server-side Blazor app, add the following endpoints for login and logout:

```csharp

// ...existing code...

app.UseAntiforgery();

// 👇 new code
app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
{
  var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
          .WithRedirectUri(returnUrl)
          .Build();

  await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/Account/Logout", async (HttpContext httpContext) =>
{
  var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
          .WithRedirectUri("/")
          .Build();

  await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
  await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});
// 👆 new code

// ...existing code...
```

### 6. Create Login and Logout components

Create `Components/Account/Login.razor` in the server-side Blazor app:

```html
<AuthorizeView>
    <Authorized>        
        <a href="Account/Logout">Log out</a>
    </Authorized>
    <NotAuthorized>
        <a href="Account/Login">Log in</a>
    </NotAuthorized>
</AuthorizeView>
```

Add the `Login.razor` component to the UI by modifying the `Components/Layout/MainLayout.razor` component of the server-side Blazor app as follows:

```html
@inherits LayoutComponentBase

<div class="page">
  
  <!-- ...existing code... -->
  
      <div class="top-row px-4">
          <Login />  <!-- 👈 new code -->
          <a href="https://learn.microsoft.com/aspnet/core/" target="_blank">About</a>
      </div>

  <!-- ...existing code... -->
</div>
```

### 7. Show the user info

Apply the changes highlighted below to the `Components/Pages/Weather.razor` component to show the user's name:

```html
@page "/weather"
@attribute [Authorize]
@attribute [StreamRendering]

<PageTitle>Weather</PageTitle>

<h1>Weather</h1>

<p>Hello @Username!</p>  <!-- 👈 new code -->
<p>This component demonstrates showing data.</p>

<!-- ...existing code... -->
  
@code {
  private WeatherForecast[]? forecasts;
  // 👇 new code
  [CascadingParameter]
  private Task<AuthenticationState>? authenticationState { get; set; }

  private string Username = "";
  // 👆 new code

    protected override async Task OnInitializedAsync()
    {
      // ...existing code...

      // 👇 new code
      if (authenticationState is not null)
      {
        var state = await authenticationState;

        Username = state?.User?.Identity?.Name ?? string.Empty;
      }
      // 👆 new code
    }

  // ...existing code...
}
```

### 8. Protect client-side Blazor components

In `Pages/Counter.razor` of the client-side Blazor app, dd the `[Authorize]` attribute to the `Counter` component to protect it from unauthenticated users:

```html
@page "/counter"
@attribute [Authorize] <!-- 👈 new code -->
@rendermode InteractiveAuto

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<!-- ...existing code... -->
```

In the `_Imports.razor` file of the client-side Blazor app, add the lines highlighted below:

```csharp
//...other using statements...
@using Microsoft.AspNetCore.Authorization

```

### 9. Show user info in client-side Blazor components

Modify the `Pages/Counter.razor` component of the client-side Blazor app to show the user's name, as in the following example:

```html
@page "/counter"
@attribute [Authorize]
@rendermode InteractiveAuto

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p>Hello @Username!</p>  <!-- 👈 new code -->

<!-- ...existing code... -->

@code {
  private int currentCount = 0;
  // 👇 new code
  [CascadingParameter]
  private Task<AuthenticationState>? authenticationState { get; set; }

  private string Username = "";

  protected override async Task OnInitializedAsync()
  {
    if (authenticationState is not null)
    {
      var state = await authenticationState;

      Username = state?.User?.Identity?.Name ?? string.Empty;
    }
    await base.OnInitializedAsync();
  }
  // 👆 new code
  
  private void IncrementCount()
  {
      currentCount++;
  }
}
```

### 10. Add support for authentication to the client-side Blazor app

Add the webassembly authentication package with the following command:

```bash
dotnet add package Microsoft.AspNetCore.Components.WebAssembly.Authentication
```

Add the line highlighted below to the `_Imports.razor` file of the client-side Blazor app:

```csharp

//...other using statements...
@using Microsoft.AspNetCore.Components.Authorization // 👈 new code
```

### 11. Sync the Authentication State

IMPORTANT: Apply only to .NET 9 or higher Blazor apps. Do NOT apply to .NET 8 Blazor apps, as this causes auth failures.

In `Program.cs` of the server-side Blazor app, add the following code to sync the authentication state between the server and client Blazor apps:

```csharp
// ...existing code...
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization; // 👈 new code

var builder = WebApplication.CreateBuilder(args);

// ...existing code...

builder.Services.AddCascadingAuthenticationState(); // 👈 new code

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();  // 👈 new code

// ...existing code...
```

In `Program.cs` of the client-side Blazor app, add the following code to sync the authentication state between the server and client Blazor apps:

```csharp
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization; // 👈 new code

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// 👇 new code
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();
// 👆 new code

await builder.Build().RunAsync();
```

### 12. Standardize TCP port

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

### 13. Test the App

Go to the server-side Blazor app's folder and run the following command:

```bash
dotnet run
```

Visit `http://localhost:5000` and click Login to start the Auth0 login flow.

