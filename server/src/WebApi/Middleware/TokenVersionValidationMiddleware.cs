using System.Security.Claims;
using Domain.Repositories;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Middleware;

public class TokenVersionValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var user = await unitOfWork.UserRepository.GetByIdAsync(new Guid(userId));
                var errorCode = user is null ? "Token.UserDeleted" : null;
                if (errorCode is null)
                {
                    if (long.TryParse(context.User.FindFirst(JwtClaimTypes.UpdatedAt)?.Value, out var tokenUpdatedAt))
                    {
                        var userUpdatedAt = new DateTimeOffset(user!.UpdatedAt).ToUnixTimeSeconds();
                        if (userUpdatedAt > tokenUpdatedAt)
                        {
                            errorCode = "Token.ExpiredData";
                        }
                    }
                }

                if (errorCode is not null)
                {
                    context.Response.StatusCode = 498; // Invalid Token (unofficial error)
                    var problemDetails = new ProblemDetails
                    {
                        Status = 498,
                        Title = "Invalid Token",
                        Extensions =
                        {
                            { "code", errorCode },
                            { "description", "Token expired, please reauthenticate." }
                        }
                    };
                    await context.Response.WriteAsJsonAsync(problemDetails);
                    return;
                }
            }
        }

        await next(context);
    }
}