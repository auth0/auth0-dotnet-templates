namespace Auth0Maui;

public partial class App : Application
{
	public App()
	{
		if (!Auth0.OidcClient.Platforms.Windows.Activator.Default.CheckRedirectionActivation())
			InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}