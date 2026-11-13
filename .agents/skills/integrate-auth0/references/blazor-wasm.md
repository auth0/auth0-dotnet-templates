# Blazor WebAssembly

Package: `Microsoft.AspNetCore.Components.WebAssembly.Authentication` (no Auth0-specific package).

`wwwroot/appsettings.json`:

```json
{ "Auth0": { "Authority": "https://{DOMAIN}", "ClientId": "{CLIENT_ID}" } }
```

`Program.cs`, from `templates/Auth0.BlazorWebAssembly/version10/Program.cs`:

```csharp
builder.Services.AddOidcAuthentication(options =>
{
  builder.Configuration.Bind("Auth0", options.ProviderOptions);
  options.ProviderOptions.ResponseType = "code";
});
```

Also needed: an `Authentication.razor` page for `/authentication/{action}`, `RemoteAuthenticatorView`, login/logout display component, and `AuthorizeRouteView` in `App.razor` (copy from the existing template and adapt to the scaffold).

.NET 8/9 used a hosted layout (Client/Server/Shared); .NET 10 is a single project. Follow whatever `dotnet new blazorwasm` produces for the target SDK. Registration: `AppType` `spa`, callback `https://localhost:5001/authentication/login-callback`.
