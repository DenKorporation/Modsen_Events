using System.Security.Claims;
using IdentityModel;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Middleware;

public class TokenVersionValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, UnitOfWork unitOfWork)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var user = await unitOfWork.UserRepository.GetByIdAsync(new Guid(userId));
                if (user != null)
                {
                    if (long.TryParse(context.User.FindFirst(JwtClaimTypes.UpdatedAt)?.Value, out var tokenUpdatedAt))
                    {
                        var userUpdatedAt = new DateTimeOffset(user.UpdatedAt).ToUnixTimeSeconds();
                        if (userUpdatedAt > tokenUpdatedAt)
                        {
                            context.Response.StatusCode = 498; // Invalid Token (unofficial error)
                            var problemDetails = new ProblemDetails
                            {
                                Status = 498,
                                Title = "Invalid Token",
                                Extensions = { { "description", "Token expired, please reauthenticate." } }
                            };
                            await context.Response.WriteAsJsonAsync(problemDetails);
                            return;
                        }
                    }
                }
            }
        }

        await next(context);
    }
}