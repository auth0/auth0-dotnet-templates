---
name: add-auth0-device-code-authorization
description: Add Auth0 device code authorization to a .NET Console application.
---

# Auth0 .NET Console Integration

Add device code authorization to a .NET Console application using `Auth0.AuthenticationApi`.

---

## Prerequisites

- .NET Console application (.NET 8 or higher)

## When NOT to Use

- **ASP.NET Core MVC applications**
- **ASP.NET Core Web APIs**
- **Blazor Web applications**
- **Single Page Applications using Blazor WebAssembly**

---

## Integration Workflow

### 1. Install SDK

```bash
dotnet add package Auth0.AuthenticationApi
```

### 2. Configure Auth0 in Program.cs

Open `Program.cs` and apply the changes highlighted below:

```csharp
// Program.cs

using System.Net.Http.Headers;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.Core.Exceptions;

var domain = "{DOMAIN}";
var clientId = "{CLIENT_ID}";

using var client = new AuthenticationApiClient(domain);

var deviceCodeResponse = await client.StartDeviceFlowAsync(new DeviceCodeRequest
{
   ClientId = clientId,
   Scope = "openid profile email",
   //Audience = "YOUR_API_IDENTIFIER"
});

Console.WriteLine("Activate this device:");
Console.WriteLine($"  Visit:      {deviceCodeResponse.VerificationUri}");
Console.WriteLine($"  Enter code: {deviceCodeResponse.UserCode}");
Console.WriteLine();
Console.WriteLine($"  Or open: {deviceCodeResponse.VerificationUriComplete}");

var pollingInterval = TimeSpan.FromSeconds(deviceCodeResponse.Interval);
AccessTokenResponse? tokenResponse = null;

Console.Write("\nWaiting for authorization");

while (tokenResponse == null)
{
   await Task.Delay(pollingInterval);
   
   try
   {
       tokenResponse = await client.GetTokenAsync(new DeviceCodeTokenRequest
       {
           ClientId = clientId,
           DeviceCode = deviceCodeResponse.DeviceCode
       });
   }
   catch (ErrorApiException ex) when (ex.ApiError?.Error == "authorization_pending")
   {
       Console.Write(".");
   }
   catch (ErrorApiException ex) when (ex.ApiError?.Error == "slow_down")
   {
       pollingInterval += TimeSpan.FromSeconds(5);
       Console.Write(".");
   }
}

Console.WriteLine("\n\nDevice authorized!");

var userInfo = await client.GetUserInfoAsync(tokenResponse.AccessToken);
Console.WriteLine($"Signed in as: {userInfo.FullName} ({userInfo.Email})");
Console.WriteLine($"ID token: {tokenResponse.IdToken[..20]}...");
Console.WriteLine($"Access token: {tokenResponse.AccessToken[..20]}...");

/*
// Now you can use the access token to call your API, for example:

using var httpClient = new HttpClient();
var request = new HttpRequestMessage(HttpMethod.Get, "https://your-api.com/endpoint");
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

var response = await httpClient.SendAsync(request);
Console.WriteLine($"\nAPI response status: {(int)response.StatusCode} {response.ReasonPhrase}");
*/
```

**IMPORTANT**: Warn the user about enabling the Device Code grant on the Auth0 dashboard and provide a link to the documentation.

### 6. Run the App

Use the following comman to run the app:

```bash
dotnet run
```
When you run the app, it will display instructions to visit a URL and enter a code to authorize the device. Once authorized, it will display the user's information and tokens.