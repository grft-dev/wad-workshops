namespace CityWeatherService;

public class CityWeather
{
    public string City { get; set; } = "";
    public string Country { get; set; } = "";
    public double Temperature { get; set; }
    public string Condition { get; set; } = "";
    public int Humidity { get; set; }
    public double Wind { get; set; }
    public string LastUpdated { get; set; } = "";
}
