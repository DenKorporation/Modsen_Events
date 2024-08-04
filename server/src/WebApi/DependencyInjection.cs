using WebApi.Middleware;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        
        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = configuration.GetRequiredSection("Identity:Url").Value;
                options.TokenValidationParameters.ValidateAudience = false;
            });
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "events");
            });
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}