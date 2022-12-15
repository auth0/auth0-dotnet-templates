## Auth0 Web API Application

For more information about creating and securing an ASP.NET Web API application with Auth0, check out the [Auth0 ASP.NET Web API Quickstart](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi).

#### Using the .NET CLI

To create a new Web API application, you can run the following command:

```
dotnet new auth0webapi [options]
```

This will create a new Web API application in the current folder. The following template-specific options are available:

- `--domain`<br>
  The Auth0 domain associated with your tenant. The default value is `yourdomain.auth0.com`.
- `--audience`<br>
  The API identifier (audience) as defined in your Auth0 dashboard. The default value is `https://your-api-id.com`
- `-f` or `--framework`<br>
  Defines the target framework to use for the .NET project. Currently, the only possible value is `net7.0`, which is also the default value.
- `--no-openaPI`<br>
  It prevents OpenAPI documentation generation (`true`). The default value is `false`.



#### Using Visual Studio for Windows

To create a new Web API application with Visual Studio, select *Auth0* from the project types dropdown list and then *Auth0 ASP.NET Core Web API*:

![Auth0 MVC Application from Visual Studio](assets/auth0-webapi-app-vs.png)

Then, after inserting the name and the folder for the project, provide the required options:

![Auth0 MVC Application options from Visual Studio](assets/auth0-webapi-app-vs-options.png)

---

[Back to README](../README.md)

