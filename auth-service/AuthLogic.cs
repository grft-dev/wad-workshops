using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Graftcode.Context;
using Microsoft.IdentityModel.Tokens;
using SharedJwt;

namespace AuthService;

public static class AuthLogic
{
    public static string[] GetFavouriteCities()
    {
        var authorization = GetAuthorizationHeader();
        var username = JwtValidator.ValidateAndGetUsername(authorization);
        return UserStore.GetFavouriteCities(username);
    }

    public static LoginResult Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new Exception("Username and password are required.");

        if (!UserStore.ValidateCredentials(username, password))
            throw new Exception("Invalid username or password.");

        return new LoginResult
        {
            Username = username,
            Token = IssueToken(username),
        };
    }

    private static string IssueToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: JwtSettings.Issuer,
            audience: JwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(JwtSettings.ExpirationHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GetAuthorizationHeader()
    {
        var context = RequestContext.Current
            ?? throw new Exception("Missing request context.");

        var authorization = context.GetHeaders()
            .FirstOrDefault(h => string.Equals(h.Key, "Authorization", StringComparison.OrdinalIgnoreCase))
            .Value;

        if (string.IsNullOrWhiteSpace(authorization))
            throw new Exception("Missing Authorization header.");

        return authorization;
    }
}
