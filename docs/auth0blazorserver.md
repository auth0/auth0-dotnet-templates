## Auth0 Blazor Server Application

For more information about creating and securing a Blazor Server application with Auth0, check out the [Auth0 Blazor Server Tutorial](https://auth0.com/blog/what-is-blazor-tutorial-on-building-webapp-with-authentication).

#### Using the .NET CLI

To create a new Blazor Server application with the .NET CLI, you can run the following command:

```
dotnet new auth0blazorserver [options]
```

This will create a new Blazor Server application in the current folder. The following template-specific options are available:

- `--domain`<br>
  The Auth0 domain associated with your tenant. The default value is `yourdomain.auth0.com`.
- `--client-id`<br>
  The client id associated with your application. The default value is `your-client-id`.
- `-f` or `--framework`<br>
  Defines the target framework to use for the .NET project. Currently, the only possible value is `net7.0`, which is also the default value.



#### Using Visual Studio for Windows

To create a new Blazor Server application with Visual Studio, select *Auth0* from the project types dropdown list and then *Auth0 Blazor Server App*:

![Auth0 Blazor Server Application from Visual Studio](assets/auth0-blazorserver-app-vs.png)

Then, after inserting the name and the folder for the project, provide the required options:

![Auth0 Blazor Server Application options from Visual Studio](assets/auth0-blazorserver-app-vs-options.png)

---

[Back to README](../README.md)

