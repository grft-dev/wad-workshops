namespace AuthService;

internal static class UserStore
{
    private static readonly Dictionary<string, string> Users = new(StringComparer.OrdinalIgnoreCase)
    {
        ["wad"] = "password",
    };

    private static readonly Dictionary<string, string[]> FavouriteCities = new(StringComparer.OrdinalIgnoreCase)
    {
        ["wad"] = new[] { "London", "Paris", "Tokyo", "New York", "Sydney", "Berlin" },
    };

    public static bool ValidateCredentials(string username, string password)
    {
        return Users.TryGetValue(username, out var storedPassword)
            && storedPassword == password;
    }

    public static string[] GetFavouriteCities(string username)
    {
        return FavouriteCities.TryGetValue(username, out var cities)
            ? cities
            : [];
    }
}
