using System.Security.Cryptography;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Repositories;
using Infrastructure.Supabase;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Supabase;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services
            .AddIdentityServer()
            .AddSigningCredential(
                GetRsaSigningCredentials(configuration.GetRequiredSection("Identity:PrivateKey").Value!))
            .AddInMemoryIdentityResources(Configuration.IdentityResources)
            .AddInMemoryApiScopes(Configuration.ApiScopes)
            .AddInMemoryClients(Configuration.Clients)
            .AddAspNetIdentity<User>();

        services.AddScoped<AppDbContextInitializer>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventUserRepository, EventUserRepository>();

        services.AddScoped<Client>(_ => new Client(
            configuration["Supabase:Url"]!,
            configuration["Supabase:Key"]!,
            new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true
            }));

        services.AddScoped<ICloudImageService, ImageService>();

        return services;
    }

    private static SigningCredentials GetRsaSigningCredentials(string privateKeyBase64)
    {
        var rsa = RSA.Create();

        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64!);
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

        return new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
    }
}