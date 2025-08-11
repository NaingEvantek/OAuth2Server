using System.Text;
using AuthServerNew.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OAuthServer.HostedServices;
using OpenIddict.Validation.AspNetCore;
using OpenIddict.Abstractions;
using static OpenIddict.Server.OpenIddictServerEvents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Configure Entity Framework Core to use Microsoft SQL Server.
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()

    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
    });
int accessTokenLifetimeMinutes = builder.Configuration.GetValue<int>("TokenSettings:AccessTokenLifetimeMinutes");
// Define your symmetric signing key here (keep it secret and secure)
var secretKeyString = builder.Configuration.GetValue<string>("TokenSettings:SigningKey");
if (string.IsNullOrEmpty(secretKeyString))
    throw new InvalidOperationException("Signing key is not configured in appsettings.json");

var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));

builder.Services.AddOpenIddict()

    // Register the OpenIddict server components.
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("auth/token");

        options.AllowClientCredentialsFlow();

        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(accessTokenLifetimeMinutes));

        options.DisableAccessTokenEncryption();

        // Use symmetric signing key instead of dev certs
       // options.AddSigningKey(secretKey);

        options
        .AddDevelopmentEncryptionCertificate()
        .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();

        options.AddEventHandler<ProcessSignInContext>(builder =>
        {
            builder.UseInlineHandler(context =>
            {
                if(context.Principal.HasClaim(claim =>
        claim.Type == OpenIddictConstants.Claims.ClientId))
{
                    context.Principal.SetAudiences("evantek-restapi");
                }
                return default;
            });
        });
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    }); 


builder.Services.AddControllers();

builder.Services.AddHostedService<Worker>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});
builder.Services.AddAuthorization();



var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseForwardedHeaders();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
