using Auth0.AspNetCore.Authentication.Api;
#if (!removeOpenAPI)
using Microsoft.OpenApi.Models;
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth0ApiAuthentication(builder.Configuration.GetSection("Auth0"));
builder.Services.AddAuthorization();

builder.Services.AddControllers();
#if (!removeOpenAPI)
builder.Services.AddSwaggerService();
#endif

var app = builder.Build();

#if (!removeOpenAPI)
if (app.Environment.IsDevelopment())
{
  app.UseStaticFiles();
  app.UseSwagger();
  app.UseSwaggerUI();
}
#endif

app.UseHttpsRedirection();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
