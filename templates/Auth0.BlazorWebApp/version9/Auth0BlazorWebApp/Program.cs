using Auth0BlazorWebApp.Client.Pages;
using Auth0BlazorWebApp.Components;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCascadingAuthenticationState();

builder.Services
    .AddAuth0WebAppAuthentication(options => {
      options.Domain = builder.Configuration["Auth0:Domain"];
      options.ClientId = builder.Configuration["Auth0:ClientId"];
    });

builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapGet("/account/login", async (HttpContext httpContext, string returnUrl = "/") =>
{
  var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
          .WithRedirectUri(returnUrl)
          .Build();

  await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/account/logout", async (HttpContext httpContext) =>
{
  var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
          .WithRedirectUri("/")
          .Build();

  await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
  await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapGet("/api/internalData", () =>
{
    var data = Enumerable.Range(1, 5).Select(index =>
        Random.Shared.Next(1, 100))
        .ToArray();

    return data;
})
.RequireAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Auth0BlazorWebApp.Client._Imports).Assembly);

app.Run();
