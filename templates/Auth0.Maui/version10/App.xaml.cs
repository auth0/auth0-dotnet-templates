using Microsoft.Extensions.DependencyInjection;

namespace Auth0Maui
{
    public partial class App : Application
    {
        public App()
        {
            #if WINDOWS
            if (Auth0.OidcClient.Platforms.Windows.Activator.Default.CheckRedirectionActivation())
                return;
            #endif
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
