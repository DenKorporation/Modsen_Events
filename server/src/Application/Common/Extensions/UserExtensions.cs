using System.Security.Claims;
using Domain.Entities;
using IdentityModel;

namespace Application.Common.Extensions;

public static class UserExtensions
{
    public static List<Claim> ToClaims(this User user)
    {
        return
        [
            new(JwtClaimTypes.GivenName, user.FirstName),
            new(JwtClaimTypes.FamilyName, user.LastName),
            new(JwtClaimTypes.Email, user.Email!),
            new(JwtClaimTypes.BirthDate, user.Birthday.ToShortDateString()),
            new(JwtClaimTypes.UpdatedAt, new DateTimeOffset(user.UpdatedAt).ToUnixTimeSeconds().ToString())
        ];
    }
}