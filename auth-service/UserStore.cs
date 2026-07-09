namespace AuthService;

internal static class UserStore
{
    private static readonly Dictionary<string, string> Users = new(StringComparer.OrdinalIgnoreCase)
    {
        ["wad"] = "password",
    };

    public static bool ValidateCredentials(string username, string password)
    {
        return Users.TryGetValue(username, out var storedPassword)
            && storedPassword == password;
    }
}
