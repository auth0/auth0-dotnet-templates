---
name: add-auth0-authentication-maui
description: Add Auth0 authentication to a .NET MAUI application, including login, logout, and user profile.
---

# Auth0 .NET MAUI Integration

Add login, logout, and user profile to a .NET MAUI application using `Auth0.OidcClient.MAUI`.

---

## Prerequisites

- .NET MAUI application (.NET 8 or higher)

## When NOT to Use

- **ASP.NET Core MVC applications**
- **ASP.NET Core Web APIs**
- **Blazor Web applications**
- **Single Page Applications using Blazor WebAssembly**
- **Console applications**

---

## Integration Workflow

### 1. Install SDK

```bash
dotnet add package Auth0.OidcClient.MAUI
```

### 2. Configure Auth0 in MauiProgram.cs

Open `MauiProgram.cs` and apply the changes highlighted below:

```csharp
// MauiProgram.cs

using Microsoft.Extensions.Logging;
using Auth0.OidcClient;    // 👈 new code

namespace MauiAuth0App;

public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
    var builder = MauiApp.CreateBuilder();
    builder
      .UseMauiApp<App>()
      .ConfigureFonts(fonts =>
      {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
      });

#if DEBUG
    builder.Logging.AddDebug();
#endif

    // 👇 new code
    builder.Services.AddSingleton<MainPage>();

    builder.Services.AddSingleton(new Auth0Client(new()
    {
      Domain = "{DOMAIN}",
      ClientId = "{CLIENT_ID}",
      RedirectUri = "myapp://callback/",
      PostLogoutRedirectUri = "myapp://callback/",
      Scope = "openid profile email"
    }));
    // 👆 new code

    return builder.Build();
  }
}
```

### 3. Update MainPage.xaml

Open `MainPage.xaml` and restructure the content to add a login button and wrap the existing content in a named `<StackLayout>`:

```xml
<!-- MainPage.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiAuth0App.MainPage">

    <ScrollView>
        <VerticalStackLayout
            Padding="30,0"
            Spacing="25">

          <!-- 👇 new code -->
          <StackLayout
              x:Name="LoginView">
              <Button
                  x:Name="LoginBtn"
                  Text="Log In"
                  SemanticProperties.Hint="Click to log in"
                  Clicked="OnLoginClicked"
                  HorizontalOptions="Center" />
          </StackLayout>
          <!-- 👆 new code -->

          <!-- 👇 new code -->
          <StackLayout
              x:Name="HomeView"
              IsVisible="false">
          <!-- 👆 new code -->

              <!-- ...existing markup... -->

              <!-- 👇 new code -->
              <Image
                  x:Name="UserPictureImg"
                  SemanticProperties.Description="User's picture"
                  HeightRequest="200"
                  HorizontalOptions="Center" />

              <Label
                  x:Name="UsernameLbl"
                  Text=""
                  SemanticProperties.HeadingLevel="Level2"
                  SemanticProperties.Description="User's name"
                  FontSize="18"
                  HorizontalOptions="Center" />

              <Button
                  x:Name="LogoutBtn"
                  Text="Log Out"
                  SemanticProperties.Hint="Click to log out"
                  Clicked="OnLogoutClicked"
                  HorizontalOptions="Center" />
              <!-- 👆 new code -->

          <!-- 👇 new code -->
          </StackLayout>
          <!-- 👆 new code -->

        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

The `LoginView` stack is shown initially (unauthenticated state). The `HomeView` stack starts hidden and becomes visible after successful login.

### 4. Update MainPage.xaml.cs

Open `MainPage.xaml.cs` and apply the changes highlighted below:

```csharp
// MainPage.xaml.cs

using Auth0.OidcClient;    // 👈 new code

namespace MauiAuth0App;

public partial class MainPage : ContentPage
{
  int count = 0;
  // 👇 new code
  private readonly Auth0Client auth0Client;
  // 👆 new code

  // 👇 changed code
  public MainPage(Auth0Client client)
  // 👆 changed code
  {
    InitializeComponent();
    auth0Client = client;    // 👈 new code
  }

  // ...existing code...

  // 👇 new code
  private async void OnLoginClicked(object sender, EventArgs e)
  {
    var loginResult = await auth0Client.LoginAsync();

    if (!loginResult.IsError)
    {
      UsernameLbl.Text = loginResult.User.Identity.Name;
      UserPictureImg.Source = loginResult.User
        .Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

      LoginView.IsVisible = false;
      HomeView.IsVisible = true;
    }
    else
    {
      await DisplayAlert("Error", loginResult.ErrorDescription, "OK");
    }
  }

  private async void OnLogoutClicked(object sender, EventArgs e)
  {
    var logoutResult = await auth0Client.LogoutAsync();

    HomeView.IsVisible = false;
    LoginView.IsVisible = true;
  }
  // 👆 new code
}
```

The `Auth0Client` is injected through the constructor (registered as a singleton in `MauiProgram.cs`). `LoginAsync()` starts the OIDC flow via the system browser. `LogoutAsync()` clears the Auth0 session and returns the user to the login view.

### 5. Apply Platform-Specific Changes

Each target platform requires additional configuration so that the system browser can redirect back to the app after authentication via `myapp://callback/`.

#### Android

Create `Platforms/Android/WebAuthenticationCallbackActivity.cs`:

```csharp
// Platforms/Android/WebAuthenticationCallbackActivity.cs

using Android.App;
using Android.Content.PM;

namespace MauiAuth0App;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { Android.Content.Intent.ActionView },
              Categories = new[] {
                Android.Content.Intent.CategoryDefault,
                Android.Content.Intent.CategoryBrowsable
              },
              DataScheme = CALLBACK_SCHEME)]
public class WebAuthenticationCallbackActivity : Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity
{
    const string CALLBACK_SCHEME = "myapp";
}
```

Open `Platforms/Android/AndroidManifest.xml` and add the `<queries>` block highlighted below:

```xml
<!-- Platforms/Android/AndroidManifest.xml -->

<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application android:allowBackup="true"
               android:icon="@mipmap/appicon"
               android:roundIcon="@mipmap/appicon_round"
               android:supportsRtl="true"></application>
    <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    <uses-permission android:name="android.permission.INTERNET" />
  <!-- 👇 new code -->
  <queries>
    <intent>
      <action android:name="android.support.customtabs.action.CustomTabsService" />
    </intent>
  </queries>
  <!-- 👆 new code -->
</manifest>
```

#### macOS (Mac Catalyst)

Open `Platforms/MacCatalyst/Info.plist` and add the `CFBundleURLTypes` key highlighted below:

```xml
<!-- Platforms/MacCatalyst/Info.plist -->

<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>

  <!-- ...existing keys... -->

  <!-- 👇 new code -->
  <key>CFBundleURLTypes</key>
  <array>
    <dict>
      <key>CFBundleURLName</key>
      <string>MauiAuth0App</string>
      <key>CFBundleURLSchemes</key>
      <array>
        <string>myapp</string>
      </array>
      <key>CFBundleTypeRole</key>
        <string>Editor</string>
    </dict>
  </array>
  <!-- 👆 new code -->

</dict>
</plist>
```

#### iOS

Add the same `CFBundleURLTypes` key to `Platforms/iOS/Info.plist`.

#### Windows

Open `Platforms/Windows/Package.appxmanifest` and add the `<Extensions>` block inside the `<Application>` element:

```xml
<!-- Platforms/Windows/Package.appxmanifest -->

<?xml version="1.0" encoding="utf-8"?>
<Package>

  <!-- ...existing markup... -->

  <Applications>
    <Application Id="App"
                 Executable="$targetnametoken$.exe"
                 EntryPoint="$targetentrypoint$">
      <uap:VisualElements
        DisplayName="$placeholder$"
        Description="$placeholder$"
        Square150x150Logo="$placeholder$.png"
        Square44x44Logo="$placeholder$.png"
        BackgroundColor="transparent">
        <uap:DefaultTile Square71x71Logo="$placeholder$.png"
                         Wide310x150Logo="$placeholder$.png"
                         Square310x310Logo="$placeholder$.png" />
        <uap:SplashScreen Image="$placeholder$.png" />
      </uap:VisualElements>
      <!-- 👇 new code -->
      <Extensions>
          <uap:Extension Category="windows.protocol">
          <uap:Protocol Name="myapp">
              <uap:DisplayName>Auth0Maui</uap:DisplayName>
          </uap:Protocol>
          </uap:Extension>
      </Extensions>
      <!-- 👆 new code -->
    </Application>
  </Applications>

  <!-- ...existing markup... -->

</Package>
```

Open `Platforms/Windows/App.xaml.cs` and add the redirection activation check highlighted below:

```csharp
using Microsoft.UI.Xaml;

namespace MauiAuth0App.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
    // 👇 new code
        if (Auth0.OidcClient.Platforms.Windows.Activator.Default.CheckRedirectionActivation())
          return;
    // 👆 new code

        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
```

### 6. Run the App

Use one of the following commands to run the app for your target platform:

```bash
# macOS
dotnet build -t:Run -f net10.0-maccatalyst

# Android
dotnet build -t:Run -f net10.0-android

# iOS
dotnet build -t:Run -f net10.0-ios

# Windows (build only; launch via Visual Studio)
dotnet build -f net10.0-windows10.0.19041.0 -p:WindowsPackageType=None
```

> **Note:** Running on Windows via the CLI is a known issue. Use Visual Studio to launch the Windows target.

After launching, the app shows only the **Log In** button. Clicking it opens the Auth0 Universal Login page in the system browser. After successful authentication, the home view appears with the user's name, picture, and a **Log Out** button.
