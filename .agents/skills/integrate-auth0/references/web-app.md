# Server web apps (MVC, Blazor Web App server, BFF server)

Package: `Auth0.AspNetCore.Authentication`.

`appsettings.json` gets:

```json
"Auth0": { "Domain": "{DOMAIN}", "ClientId": "{CLIENT_ID}" }
```

`Program.cs` (MVC), modelled on `templates/Auth0.Mvc/version10/Program.cs`:

```csharp
builder.Services.AddAuth0WebAppAuthentication(options => {
  options.Domain = builder.Configuration["Auth0:Domain"];
  options.ClientId = builder.Configuration["Auth0:ClientId"];
  options.Scope = "openid profile email";
});
...
app.UseAuthentication();
app.UseAuthorization();
```

Also needed (copy from the existing template of the same type and adapt to the new framework's scaffold):
- Login and logout actions using `LoginAuthenticationPropertiesBuilder` / `LogoutAuthenticationPropertiesBuilder`, signing out of both `Auth0Constants.AuthenticationScheme` and the cookie scheme.
- A profile page showing the user's claims, protected with `[Authorize]`.
- Login/logout links in the layout.

Blazor Web App additionally: `AddCascadingAuthenticationState()`, `AddAuthenticationStateSerialization()` on the Razor components builder, `/account/login` and `/account/logout` minimal endpoints, and a protected endpoint for the client project to call. See `templates/Auth0.BlazorWebApp/version10/Auth0BlazorWebApp/Program.cs`.

Register callbacks later in `registration/config.json`: `https://localhost:<httpsPort>/callback` and `https://localhost:<httpsPort>/`.
