using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Authorization.Requirements;

namespace WebApi.Authorization.Handlers;

public class UserIdMatchingHandler : AuthorizationHandler<UserIdMatchingRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        UserIdMatchingRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var routeData = context.Resource as DefaultHttpContext;
        var userId = routeData?.HttpContext.Request.RouteValues["userId"]?.ToString();

        if (userIdClaim.Value == userId)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}