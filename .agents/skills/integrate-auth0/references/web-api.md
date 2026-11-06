# Web API (authorization with access tokens)

Package: `Microsoft.AspNetCore.Authentication.JwtBearer` (plus `Swashbuckle.AspNetCore` and `Microsoft.AspNetCore.OpenApi`, conditional on `removeOpenAPI`, as in the existing csproj).

`appsettings.json`, modelled on `templates/Auth0.MinimalWebAPI/version10/appsettings.json`:

```json
"Authentication": {
  "Schemes": {
    "Bearer": {
      "Authority": "https://{DOMAIN}",
      "ValidAudiences": [ "{AUDIENCE}" ],
      "ValidIssuer": "{DOMAIN}"
    }
  }
}
```

`Program.cs`:

```csharp
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();
...
app.MapGet("/weatherforecast", ...).RequireAuthorization();   // minimal
// controllers: app.MapControllers(); and [Authorize] on the controller
```

Keep the `#if (!removeOpenAPI)` blocks and `SwaggerExtensions.cs` from the existing templates (it adds the bearer security definition to Swagger UI). Minimal API is the default variant; the controller variant lives in `Auth0.WebAPI/version<N>`.

Registration: `AppType` `api`, empty `Callbacks` and `LogoutUrls`.
