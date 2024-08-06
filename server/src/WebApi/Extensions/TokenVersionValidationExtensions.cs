using WebApi.Middleware;

namespace WebApi.Extensions;

public static class TokenVersionValidationExtensions
{
    public static IApplicationBuilder UseTokenVersionValidation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TokenVersionValidationMiddleware>();
    }
}