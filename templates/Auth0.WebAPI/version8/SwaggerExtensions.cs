using Microsoft.OpenApi.Models;

public static class SwaggerExtensions
{
  public static IServiceCollection AddSwaggerService(this IServiceCollection services)
  {
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Auth0WebAPI",
        Description = "Learn how to protect your .NET applications with Auth0",
        Contact = new OpenApiContact {
          Name = ".NET Identity with Auth0",
          Url = new Uri("https://a0.to/dotnet-templates/webapi")
        },
        Version = "v1.0.0" });

      var securitySchema = new OpenApiSecurityScheme
      {
        Description = "Using the Authorization header with the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        }
      };

      options.AddSecurityDefinition("Bearer", securitySchema);

      options.AddSecurityRequirement(new OpenApiSecurityRequirement
              {
                  { securitySchema, new[] { "Bearer" } }
              });
    });

    return services;
  }
}