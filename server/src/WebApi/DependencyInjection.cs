using System.Security.Cryptography;
using Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Authorization.Handlers;
using WebApi.Authorization.Requirements;
using WebApi.Middleware;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidIssuer = configuration.GetRequiredSection("Identity:Url").Value,
                    IssuerSigningKey = GetRsaPublicKey(configuration.GetRequiredSection("Identity:PublicKey").Value!)
                };
            });

        services.AddAuthorization(ConfigureAuthorizationOption);

        services.AddSingleton<IAuthorizationHandler, UserIdMatchingHandler>();
        services.AddSingleton<IAuthorizationHandler, UserIdMatchingOrAdminRoleHandler>();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        var tokenUri = new Uri(configuration.GetRequiredSection("Identity:Url").Value + "/connect/token");
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Events API", Version = "v1" });
            options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        RefreshUrl = tokenUri,
                        TokenUrl = tokenUri,
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "openid" },
                            { "profile", "profile" },
                            { "events", "events " },
                            { "offline_access", "offline_access " },
                        }
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "OAuth2"
                        },
                        Name = "Bearer",
                        Scheme = "Bearer",
                    },
                    new string[] { }
                }
            });
        });

        services.AddCors(corsOptions =>
        {
            corsOptions.AddPolicy("Angular Client",
                policy =>
                {
                    policy
                        .WithOrigins(configuration.GetRequiredSection("Clients:AngularUrl").Value!)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        return services;
    }

    private static RsaSecurityKey GetRsaPublicKey(string publicKeyBase64)
    {
        var rsa = RSA.Create();

        var publicKeyBytes = Convert.FromBase64String(publicKeyBase64!);
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        return new RsaSecurityKey(rsa);
    }

    private static void ConfigureAuthorizationOption(AuthorizationOptions options)
    {
        options.AddPolicy(Policies.ApiScope, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope", "events");
        });

        options.AddPolicy(Policies.AdminRole, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(Roles.Administrator);
        });

        options.AddPolicy(Policies.ReadEvent, policy => { policy.RequireAuthenticatedUser(); });

        options.AddPolicy(Policies.ModifyEvent, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(Roles.Administrator);
        });

        options.AddPolicy(Policies.UserIdMatching, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new UserIdMatchingRequirement());
        });

        options.AddPolicy(Policies.UserIdMatchingOrAdminRole, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new UserIdMatchingOrAdminRoleRequirement());
        });
    }
}