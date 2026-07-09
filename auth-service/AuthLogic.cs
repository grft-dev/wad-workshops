using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SharedJwt;

namespace AuthService;

public static class AuthLogic
{
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
}
