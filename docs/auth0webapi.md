## Auth0 Web API Application

For more information about creating and securing an ASP.NET Web API application with Auth0, check out the [Auth0 ASP.NET Web API Quickstart](https://auth0.com/docs/quickstart/backend/aspnet-core-webapi).

#### Using the .NET CLI

To create a new Web API application, you can run the following command:

```
dotnet new auth0webapi [options]
```

This will create a new Web API application with Auth0 support in the current folder.

##### Automatic registration

If you have the [Auth0 CLI](https://github.com/auth0/auth0-cli) installed on your machine and [logged in to Auth0](https://github.com/auth0/auth0-cli?tab=readme-ov-file#authenticating-to-your-tenant), you can run the template command without any options and it will automatically register and configure your application with Auth0.

Example:

```shell
dotnet new auth0webapi -o MyWebAPI
```

The template engine will ask for confirmation to perform the registration action:

```shell
The template "Auth0 ASP.NET Core Web API" was created successfully.

Processing post-creation actions...

Template is configured to run the following action:
Actual command: register-with-auth0.cmd 
Do you want to run this action [Y(yes)|N(no)]?
```

Once you confirm, you will get an entry for the application **in your current Auth0 tenant** and your application will be configured accordingly.

##### Manual registration

In addition to the usual options for the `dotnet new` command, the following template-specific options are available:

- `--domain`<br>
  The Auth0 domain associated with your tenant. The default value is `yourdomain.auth0.com`.
- `--audience`<br>
  The API identifier (audience) as defined in your Auth0 dashboard. The default value is `https://your-api-id.com`
- `-f` or `--framework`<br>
  Defines the target framework to use for the .NET project. Currently, the only possible value is `net7.0`, which is also the default value.
- `--no-openapi`<br>
  It prevents OpenAPI documentation generation (`true`). The default value is `false`.
- `--minimal`
  It creates a project that uses the [ASP.NET Core minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis). Default is `false`.

Example:

```shell
dotnet new auth0webapi -o MyWebAPI --domain myapp.auth0.com --audience https://myapi.com
```

#### Using Visual Studio for Windows

To create a new Web API application with Visual Studio for Windows, select *Auth0* from the project types dropdown list and then *Auth0 ASP.NET Core Web API*:

![Auth0 MVC Application from Visual Studio](assets/auth0-webapi-app-vs.png)

Then, after inserting the name and the folder for the project, provide the required options:

![Auth0 MVC Application options from Visual Studio](assets/auth0-webapi-app-vs-options.png)

#### Using Visual Studio for Mac

To create a new Web API application with Visual Studio for Mac, select *Custom* from the *Other* project types list and then *Auth0 ASP.NET Core Web API*:

![Auth0 MVC Application from Visual Studio](assets/auth0-webapi-app-vs-mac.png)

Then, after inserting the name and the folder for the project, provide the required options:

![Auth0 MVC Application options from Visual Studio](assets/auth0-webapi-app-vs-mac-options.png)

##### Automatic registration

Unfortunately, Visual Studio does not support template's post actions (see [this issue](https://github.com/dotnet/templating/issues/4575) and [this one](https://github.com/dotnet/templating/issues/3226)) so your application will not be automatically registered as it happens with .NET CLI. However, you can run the post action manually to get your application configured.

To launch the automatic registration process, go to the folder of the newly created application and run the following command:

```shell
./register-with-auth0.cmd
```

> Note: on MacOS you need to enable the script to execute. Run the following command before launching the automatic registration:
>
> ```shell
> chmod +x register-with-auth0.cmd
> ```


---

[Back to README](../README.md)

