using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth0WebAppAuthentication(options =>
{
  options.Domain = builder.Configuration["Auth0:Domain"];
  options.ClientId = builder.Configuration["Auth0:ClientId"];
  options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
})
  .WithAccessToken(options =>
  {
    options.Audience = builder.Configuration["Auth0:ApiAudience"];
  });

builder.Services.AddAuthorization();

builder.Services.AddHttpClient("WeatherApi", client =>
{
  client.BaseAddress = new Uri(builder.Configuration["WeatherApiBaseUrl"]
    ?? throw new InvalidOperationException("WeatherApiEndpoint is not configured"));
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/bff/login", async (HttpContext httpContext, string returnUrl = "/") =>
{
  var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
          .WithRedirectUri(returnUrl)
          .Build();

  await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/bff/logout", async (HttpContext httpContext) =>
{
  var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
          .WithRedirectUri("/")
          .Build();

  await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
  await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}).RequireAuthorization();

app.MapGet("/bff/getUser", (HttpContext httpcontext) =>
{
  var result = new { isAuthenticated = false, claims = new[] { new { type = "", value = "" } } };

  if (httpcontext.User.Identity?.IsAuthenticated == true)
  {
    var claims = ((ClaimsIdentity)httpcontext.User.Identity!).Claims
      .Select(c => new { type = c.Type, value = c.Value })
      .ToArray();

    result = new { isAuthenticated = true, claims };
  }

  return (object)result;
});

app.MapGet("/bff/weatherforecast", async Task<IResult> (
  HttpContext httpContext, 
  IHttpClientFactory httpClientFactory) =>
{
  var httpClient = httpClientFactory.CreateClient("WeatherApi");

  var accessToken = await httpContext.GetTokenAsync(Auth0Constants.AuthenticationScheme, "access_token");

  if (string.IsNullOrEmpty(accessToken))
    return Results.Unauthorized();

  var request = new HttpRequestMessage(HttpMethod.Get, "WeatherForecast");
  request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

  var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

  if (!response.IsSuccessStatusCode)
  {
    response.Dispose();
    return Results.StatusCode((int)response.StatusCode);
  }

  var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";
  return Results.Stream(await response.Content.ReadAsStreamAsync(), contentType);
})
.RequireAuthorization();

app.MapFallbackToFile("/index.html");

app.Run();

