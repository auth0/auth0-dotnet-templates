# Banner placement by project type

Image source: `templates/Auth0.Mvc/version10/wwwroot/images/auth0-dotnet-ebook.png`. Link base: `https://a0.to/dotnet-templates/`.

| Project type | Slug | Image destination | Link/markup location |
|---|---|---|---|
| MVC (`Auth0.Mvc`) | `mvc` | `wwwroot/images/auth0-dotnet-ebook.png` | `Views/Home/Index.cshtml` |
| Web API / minimal API (`Auth0.WebAPI`, `Auth0.MinimalWebAPI`) | `webapi` | `wwwroot/images/auth0-dotnet-ebook.png` | `wwwroot/index.html` and `SwaggerExtensions.cs` (`Contact.Url`) |
| Blazor Web App (`Auth0BlazorWebApp`) | `blazor-webapp` | `Auth0BlazorWebApp/wwwroot/images/auth0-dotnet-ebook.png` | `Components/Pages/Home.razor` |
| Blazor WebAssembly | `blazor-wasm` | `wwwroot/images/auth0-dotnet-ebook.png` (`Client/wwwroot/images/` for the .NET 8/9 hosted layout) | `Pages/Home.razor` and the `NotAuthorized` block of `App.razor` |
| Blazor Server (discontinued) | `blazor-server` | `wwwroot/images/` | `App.razor`; do not extend |
| MAUI | `maui` | `Resources/Images/auth0_dotnet_ebook.png` | `MainPage.xaml` (`Image`) and `MainPage.xaml.cs` (`Launcher.OpenAsync`) |
| BFF client (React/Vite) | `bff` | `src/assets/auth0-dotnet-ebook.png` | `src/App.jsx` |
| BFF API project (`WeatherForecastAPI`) | `webapi` | `wwwroot/images/auth0-dotnet-ebook.png` | `wwwroot/index.html`, `SwaggerExtensions.cs` |
| Console | none | none | **no banner** |
| Any other type | ask the user | ask | ask |

## Snippets

Razor / MVC / HTML (inside a centered wrapper):

```html
<div class="text-center">
  <div>
    <a href="https://a0.to/dotnet-templates/<slug>"><img src="images/auth0-dotnet-ebook.png" alt=".NET Identity with Auth0"></a>
  </div>
</div>
```

Swagger (`SwaggerExtensions.cs`):

```csharp
Contact = new() {
  Name = ".NET Identity with Auth0",
  Url = new Uri("https://a0.to/dotnet-templates/webapi")
},
```

MAUI, `MainPage.xaml`, as the first child of the `VerticalStackLayout`:

```xml
<Image Source="auth0_dotnet_ebook.png">
  <Image.GestureRecognizers>
    <TapGestureRecognizer Tapped="OnAuth0BookClicked"/>
  </Image.GestureRecognizers>
</Image>
```

MAUI, `MainPage.xaml.cs`:

```csharp
private async void OnAuth0BookClicked(object? sender, Microsoft.Maui.Controls.TappedEventArgs e)
{
  await Launcher.OpenAsync("https://a0.to/dotnet-templates/maui");
}
```

React (BFF client, `App.jsx`):

```jsx
<div>
  <a href="https://a0.to/dotnet-templates/bff"><img src="src/assets/auth0-dotnet-ebook.png" alt=".NET Identity with Auth0"/></a>
</div>
```

Live examples to compare against: `templates/Auth0.Mvc/version10/Views/Home/Index.cshtml`, `templates/Auth0.MinimalWebAPI/version10/wwwroot/index.html`, `templates/Auth0.Maui/version10/MainPage.xaml`.
