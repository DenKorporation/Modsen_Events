using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization.Requirements;

public record UserIdMatchingRequirement : IAuthorizationRequirement;