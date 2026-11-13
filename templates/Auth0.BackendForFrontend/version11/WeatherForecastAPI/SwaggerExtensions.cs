using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public static class SwaggerExtensions
{
  public static IServiceCollection AddOpenApiService(this IServiceCollection services)
  {
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    services.AddOpenApi(options =>
    {
      options.AddDocumentTransformer((document, context, cancellationToken) =>
      {
        document.Info = new OpenApiInfo
        {
          Title = "WeatherForecastAPI",
          Description = "Learn how to protect your .NET applications with Auth0",
          Contact = new OpenApiContact
          {
            Name = ".NET Identity with Auth0",
            Url = new Uri("https://a0.to/dotnet-templates/webapi")
          },
          Version = "v1.0.0"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
          Description = "Using the Authorization header with the Bearer scheme.",
          Name = "Authorization",
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.Http,
          Scheme = "bearer",
          BearerFormat = "JWT"
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
          [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

        return Task.CompletedTask;
      });
    });

    return services;
  }
}
