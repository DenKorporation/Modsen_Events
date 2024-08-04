using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace Infrastructure.Identity;

public static class Configuration
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
            {
                UserClaims = new List<string>
                {
                    JwtClaimTypes.GivenName,
                    JwtClaimTypes.FamilyName,
                    JwtClaimTypes.Email,
                    JwtClaimTypes.Role,
                    JwtClaimTypes.BirthDate,
                    JwtClaimTypes.UpdatedAt
                }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope(
                name: "events",
                displayName: "Events API",
                userClaims:
                [
                    JwtClaimTypes.Role,
                    JwtClaimTypes.UpdatedAt
                ])
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            new Client
            {
                ClientId = "angular",
                ClientName = "Angular Client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                RequireClientSecret = false,
                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "events"
                }
            }
        };
}