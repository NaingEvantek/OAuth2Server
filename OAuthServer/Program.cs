using System.Security.Cryptography.X509Certificates;
using System.Text;
using AuthServerNew.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OAuthServer.HostedServices;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
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
string issuer = builder.Configuration.GetValue<string>("TokenSettings:Issuer") ?? "";

// Symmetric signing key
var secretKeyString = builder.Configuration.GetValue<string>("TokenSettings:SigningKey");
if (string.IsNullOrEmpty(secretKeyString))
    throw new InvalidOperationException("Signing key is not configured in appsettings.json");
var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyString));

// -------------------------
// Load certificate from PFX for production
// -------------------------
X509Certificate2? certificate = null;

if (!builder.Environment.IsDevelopment())
{
    var pfxPath = builder.Configuration.GetValue<string>("HTTPS_CERTIFICATE:PFX_PATH");
    var pfxPassword = builder.Configuration.GetValue<string>("HTTPS_CERTIFICATE:PFX_PASSWORD");

    if (string.IsNullOrEmpty(pfxPath))
        throw new InvalidOperationException("PFX certificate path is not configured.");

    certificate = new X509Certificate2(pfxPath, pfxPassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
}

builder.Services.AddOpenIddict()
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("auth/token");
        options.AllowClientCredentialsFlow();
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(accessTokenLifetimeMinutes));
        options.DisableAccessTokenEncryption();

        // Add symmetric key
        options.AddSigningKey(secretKey);

        // Add PFX certificate for production
        if (certificate != null)
        {
            options.AddSigningCertificate(certificate);
            options.AddEncryptionCertificate(certificate);
        }

        // Development certificates
        if (builder.Environment.IsDevelopment())
        {
            options.AddDevelopmentEncryptionCertificate();
            options.AddDevelopmentSigningCertificate();
        }

        options.SetIssuer(new Uri(issuer));

        options.UseAspNetCore()
               .DisableTransportSecurityRequirement()
               .EnableTokenEndpointPassthrough();

        options.AddEventHandler<ProcessSignInContext>(builder =>
        {
            builder.UseInlineHandler(context =>
            {
                if (context.Principal.HasClaim(claim => claim.Type == OpenIddictConstants.Claims.ClientId))
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "OAuth API",
        Version = "v1",
        Description = "For Pweb online or Treasure!"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();
app.UseForwardedHeaders();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.Run();
