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
