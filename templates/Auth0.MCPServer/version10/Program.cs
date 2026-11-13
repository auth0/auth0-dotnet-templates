using Auth0.AspNetCore.Authentication.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ModelContextProtocol.AspNetCore.Authentication;
using ModelContextProtocol.Authentication;

var builder = WebApplication.CreateBuilder(args);

var domain = builder.Configuration["Auth0:Domain"];
var audience = builder.Configuration["Auth0:Audience"];
var authority = $"https://{domain}/";

var authentication = builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
});

authentication.AddAuth0ApiAuthentication(builder.Configuration.GetSection("Auth0"));

authentication.AddMcp(options =>
    {
        // Protected Resource Metadata (RFC 9728), published at /.well-known/oauth-protected-resource
        options.ResourceMetadata = new ProtectedResourceMetadata
        {
            Resource = audience!,
            AuthorizationServers = { authority },
            ScopesSupported = [McpPermissions.GetSystemState, McpPermissions.SetSystemState]
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(McpPermissions.GetSystemStatePolicy, policy =>
        policy.RequireClaim("permissions", McpPermissions.GetSystemState));
    options.AddPolicy(McpPermissions.SetSystemStatePolicy, policy =>
        policy.RequireClaim("permissions", McpPermissions.SetSystemState));
});

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        // Stateless mode is recommended for servers that don't need
        // server-to-client requests like sampling or elicitation.
        options.Stateless = true;
    })
    .AddAuthorizationFilters()
    .WithTools<RandomNumberTools>()
    .WithTools<SystemTools>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapGet("/", () => Results.File(
    Path.Combine(app.Environment.WebRootPath, "index.html"), "text/html"));

app.MapMcp("/mcp").RequireAuthorization();

app.Run();
