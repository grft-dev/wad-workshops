using Graftcode.Context;
using Graft = graft.nuget.WeatherService;
using SharedJwt;

namespace CityWeatherService;

public static class WeatherLogic
{
    private const string UpstreamHost = "wss://dotnetweatherapi.onrender.com/ws";

    static WeatherLogic()
    {
        Graft.GraftConfig.Host = UpstreamHost;
        Graft.GraftConfig.Stateless = true;
    }

    public static CityWeather GetWeather(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            throw new Exception("City is required.");

        RequireAuthenticatedCaller();

        var forecast = Graft.WeatherProvider.GetWeatherForecast(city, 1, "en");
        var location = forecast.Location;
        var current = forecast.Current;
        var condition = current.Condition;

        return new CityWeather
        {
            City = location.Name ?? city,
            Country = location.Country ?? "",
            Temperature = current.TempC,
            Condition = condition?.Text ?? "",
            Humidity = current.Humidity,
            Wind = current.WindKph,
            LastUpdated = current.LastUpdated ?? "",
        };
    }

    private static void RequireAuthenticatedCaller()
    {
        var context = RequestContext.Current
            ?? throw new Exception("Missing request context.");

        var authorization = context.GetHeaders()
            .FirstOrDefault(h => string.Equals(h.Key, "Authorization", StringComparison.OrdinalIgnoreCase))
            .Value;

        JwtValidator.ValidateAndGetUsername(authorization ?? "");
    }
}
