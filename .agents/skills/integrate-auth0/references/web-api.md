# Web API (authorization with access tokens)

Package: `Auth0.AspNetCore.Authentication.Api` (the [Auth0 ASP.NET Core API SDK](https://github.com/auth0/aspnetcore-api), a drop-in replacement for `Microsoft.AspNetCore.Authentication.JwtBearer`; supports .NET 8 and above). Do not add `Microsoft.AspNetCore.Authentication.JwtBearer`. Keep `Swashbuckle.AspNetCore` and `Microsoft.AspNetCore.OpenApi`, conditional on `removeOpenAPI`, as in the existing csproj.

`appsettings.json`:

```json
"Auth0": {
  "Domain": "{DOMAIN}",
  "Audience": "{AUDIENCE}"
}
```

`Domain` has no `https://` prefix; the SDK builds the authority (`https://{Domain}`) itself. Do not add an `Authentication:Schemes:Bearer` section.

`Program.cs`:

```csharp
using Auth0.AspNetCore.Authentication.Api;
...
builder.Services.AddAuth0ApiAuthentication(builder.Configuration.GetSection("Auth0"));
builder.Services.AddAuthorization();
...
app.UseAuthentication();
app.UseAuthorization();
...
app.MapGet("/weatherforecast", ...).RequireAuthorization();   // minimal
// controllers: app.MapControllers(); and [Authorize] on the controller
```

Keep the `#if (!removeOpenAPI)` blocks and `SwaggerExtensions.cs` from the existing templates (it adds the bearer security definition to Swagger UI). Minimal API is the default variant; the controller variant lives in `Auth0.WebAPI/version<N>`.

DPoP is optional and off by default; to enable it chain `.WithDPoP()` on `AddAuth0ApiAuthentication(...)` (modes: `DPoPModes.Allowed` (default), `Required`, `Disabled`).

Registration: `AppType` `api`, empty `Callbacks` and `LogoutUrls`.
