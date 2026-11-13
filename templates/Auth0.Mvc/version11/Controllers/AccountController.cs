using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Auth0Mvc.ViewModels;

public class AccountController : Controller
{
    public async Task Login(string returnUrl = "/")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

  [Authorize]
  public async Task Logout()
  {
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
        .WithRedirectUri(Url.Action("Index", "Home"))
        .Build();

    await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
  }

  [Authorize]
  public IActionResult Profile()
  {
    return View(new UserProfileViewModel()
    {
      Name = User.Identity?.Name,
      EmailAddress = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value,
      ProfileImage = User.FindFirst(c => c.Type == "picture")?.Value
    });
  }
}