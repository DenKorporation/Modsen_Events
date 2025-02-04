using System.Globalization;
using System.Security.Claims;
using Application.Common.Extensions;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Supabase.Storage;
using Supabase.Storage.Interfaces;
using Client = Supabase.Client;

namespace Infrastructure.Data;

public static class InitializerExtensions
{
    public static async Task InitialiseDatabaseAsync(this IHost app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

        await initializer.InitializeAsync(cancellationToken);

        await initializer.SeedAsync(cancellationToken);
    }
}

public class AppDbContextInitializer(
    ILogger<AppDbContextInitializer> logger,
    IConfiguration configuration,
    AppDbContext context,
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    Client supabaseClient)
{
    private readonly IStorageClient<Bucket, FileObject> _storageClient = supabaseClient.Storage;
    private readonly string _previewsBucket = configuration.GetRequiredSection("Storage:PreviewsBucket").Value!;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await context.Database.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await TrySeedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync(CancellationToken cancellationToken = default)
    {
        if (!roleManager.Roles.Any())
        {
            foreach (var role in GetPreconfiguredRoles())
            {
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }

        // Default users
        if (!userManager.Users.Any())
        {
            foreach (var user in GetPreconfiguredUsers())
            {
                var result = await userManager.CreateAsync(user, "Pass123$");
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userManager.AddToRoleAsync(user, Roles.Administrator);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userManager.AddClaimsAsync(user, user.ToClaims());
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }

        // Default data
        if (!context.Events.Any())
        {
            await _storageClient.EmptyBucket(_previewsBucket);
            await context.Events.AddRangeAsync(GetPreconfiguredEvents(), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static IEnumerable<IdentityRole<Guid>> GetPreconfiguredRoles()
    {
        return
        [
            new() { Name = Roles.Administrator },
            new() { Name = Roles.Registered }
        ];
    }

    private static IEnumerable<User> GetPreconfiguredUsers()
    {
        return
        [
            new User
            {
                Id = new Guid("FF508811-60EA-426C-A2D1-1D43A95A9FD8"),
                FirstName = "FirstName",
                LastName = "LastName",
                Birthday = DateOnly.ParseExact("2000-01-01", "yyyy-MM-dd"),
                Email = "admin@example.com",
                UserName = "admin@example.com",
                UpdatedAt = DateTime.UtcNow
            }
        ];
    }

    private static IEnumerable<Event> GetPreconfiguredEvents()
    {
        return
        [
            new Event
            {
                Id = new Guid("C5E96700-BA55-497D-99F4-6AD9409D19B1"),
                Name = "Test Event",
                Description = "Test Event Description",
                Date = DateTime.ParseExact("2024-08-30 13:30", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                Address = "Test Address",
                Category = "Test Category",
                Capacity = 100,
                ImageStoragePath = null,
                ImageUrl = null,
                EventUsers =
                [
                    new EventUser
                    {
                        UserId = new Guid("FF508811-60EA-426C-A2D1-1D43A95A9FD8"),
                        RegistrationDate = DateOnly.FromDateTime(DateTime.Today)
                    }
                ]
            }
        ];
    }
}