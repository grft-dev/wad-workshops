namespace SharedJwt;

/// <summary>
/// Shared JWT configuration used by all workshop services. Keep these values identical
/// wherever tokens are issued or validated.
/// </summary>
public static class JwtSettings
{
    public const string SigningKey = "wad-workshop-signing-key-min-32-chars!";
    public const string Issuer = "wad-workshop";
    public const string Audience = "wad-workshop-clients";
    public const int ExpirationHours = 24;
}
