using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization.Requirements;

public record UserIdMatchingOrAdminRoleRequirement : IAuthorizationRequirement;