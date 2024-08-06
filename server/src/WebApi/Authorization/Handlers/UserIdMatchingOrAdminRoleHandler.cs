using System.Security.Claims;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using WebApi.Authorization.Requirements;

namespace WebApi.Authorization.Handlers;

public class UserIdMatchingOrAdminRoleHandler : AuthorizationHandler<UserIdMatchingOrAdminRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        UserIdMatchingOrAdminRoleRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        var isAdmin = context.User.IsInRole(Roles.Administrator);

        if (userIdClaim == null && !isAdmin)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var routeData = context.Resource as DefaultHttpContext;
        var userId = routeData?.HttpContext.Request.RouteValues["userId"]?.ToString();

        if (isAdmin || (userIdClaim != null && userIdClaim.Value == userId))
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