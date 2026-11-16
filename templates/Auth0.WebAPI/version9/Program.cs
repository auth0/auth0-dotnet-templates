using Auth0.AspNetCore.Authentication.Api;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth0ApiAuthentication(builder.Configuration.GetSection("Auth0"));
builder.Services.AddAuthorization();

builder.Services.AddControllers();
#if (!removeOpenAPI)
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerService();
#endif

var app = builder.Build();

#if (!removeOpenAPI)
if (app.Environment.IsDevelopment())
{
  app.UseStaticFiles();
  app.MapOpenApi();
  app.UseSwagger();
  app.UseSwaggerUI();
}
#endif

app.UseHttpsRedirection();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
