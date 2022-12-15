using Microsoft.AspNetCore.Authentication.JwtBearer;
#if (!removeOpenAPI)
using Microsoft.OpenApi.Models;
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddControllers();
#if (!removeOpenAPI)
builder.Services.AddSwaggerService();
#endif

var app = builder.Build();

#if (!removeOpenAPI)
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}
#endif

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
