using System.Globalization;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

public class AppDbContextInitializer(ILogger<AppDbContextInitializer> logger, AppDbContext context)
{
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
        // Default users
        if (!context.Users.Any())
        {
            await context.Users.AddRangeAsync(GetPreconfiguredUsers(), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }

        // Default data
        if (!context.Events.Any())
        {
            await context.Events.AddRangeAsync(GetPreconfiguredEvents(), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
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
                Email = "test@example.com"
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
                ImageStoragePath = "Previews/C5E96700-BA55-497D-99F4-6AD9409D19B1/preview.jpg",
                ImageUrl = "https://qkmmxxtecbgyplwbwzxr.supabase.co/storage/v1/object/public/Previews/C5E96700-BA55-497D-99F4-6AD9409D19B1/preview.jpg",
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