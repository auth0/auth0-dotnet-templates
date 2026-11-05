---
name: add-auth0-minimal-api
description: Secures an ASP.NET Core Minimal Web API with Auth0.
---

# Secure ASP.NET Core Minimal APIs with Auth0

Protect a .NET minimal Web API by validating Auth0-issued JWT access tokens.

## When to Use

- Adding Auth0 authentication to an ASP.NET Core minimal API (.NET 8 or higher).
- Protecting endpoints with `.RequireAuthorization()`.

## When NOT to Use

- **ASP.NET Core MVC applications**
- **Blazor Web applications**
- **Single Page Applications using Blazor WebAssembly**
- **Console Applications**

## Prerequisites

- ASP.NET Core minimal API application (.NET 8 or higher)

## Procedure

### 1. Install SDK

```bash
dotnet add package Auth0.AspNetCore.Authentication.Api
```

## 2. Configure `appsettings.json`

Add an `Auth0` section:

```json
"Auth0": {
  "Domain": "{DOMAIN}",
  "Audience": "{AUDIENCE}"
}
```

### 4. Configure Auth0 in `Program.cs`

```csharp
using Auth0.AspNetCore.Authentication.Api;

builder.Services.AddAuth0ApiAuthentication(
    builder.Configuration.GetSection("Auth0"));
builder.Services.AddAuthorization();
```

### 5. Protect endpoints

Chain `.RequireAuthorization()` on the endpoint:

```csharp
app.MapGet("/weatherforecast", () => { /* ... */ })
   .WithName("GetWeatherForecast")
   .RequireAuthorization();
```

### 6. Run the App

Use the following command to run the app:

```bash
dotnet run
```
Use `curl` to send requests.

1. Without a token, expect 401:
   ```
   curl https://localhost:7195/weatherforecast -v
   ```
   The response has `www-authenticate: Bearer` and an empty body.

2. With a token:

   ```
   curl https://localhost:7195/weatherforecast -H 'Authorization: Bearer YOUR_ACCESS_TOKEN'
   ```

Use the base address printed by `dotnet run`.
