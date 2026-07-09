using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SharedJwt;

public static class JwtValidator
{
    public static string ValidateAndGetUsername(string authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
            throw new Exception("Missing Authorization header.");

        var token = authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authorizationHeader["Bearer ".Length..].Trim()
            : authorizationHeader.Trim();

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("Missing bearer token.");

        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JwtSettings.Issuer,
            ValidAudience = JwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.SigningKey)),
        };

        try
        {
            var principal = handler.ValidateToken(token, validationParameters, out _);
            var username = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("Invalid token: missing subject claim.");

            return username;
        }
        catch (SecurityTokenException)
        {
            throw new Exception("Invalid or expired token.");
        }
    }
}
