# MAUI and console (native apps)

## MAUI
Package: `Auth0.OidcClient.MAUI`. No settings file: placeholders live in `MauiProgram.cs`.

```csharp
builder.Services.AddSingleton(new Auth0Client(new()
{
  Domain = "{DOMAIN}",
  ClientId = "{CLIENT_ID}",
  RedirectUri = "myapp://callback/",
  PostLogoutRedirectUri = "myapp://callback/",
  Scope = "openid profile email"
}));
```

Also needed: the `myapp` URL scheme handling on Android (`WebAuthenticatorActivity` with an intent filter) and the platform files, plus login/logout buttons in `MainPage`. Copy from `templates/Auth0.Maui/version10` and adapt to the new MAUI scaffold. Requires the MAUI workload; check `dotnet workload list`.

## Console (device authorization flow)
Package: `Auth0.AuthenticationApi`. Placeholders are string literals at the top of `Program.cs`:

```csharp
var domain = "{DOMAIN}";
var clientId = "{CLIENT_ID}";
```

The flow (start device flow, show code, poll for the token, fetch userinfo) is in `templates/Auth0.Console/version10/Program.cs`; reuse it as is unless the SDK API changed. Registration needs `GrantTypes` `code,implicit,refresh-token,device-code`.
