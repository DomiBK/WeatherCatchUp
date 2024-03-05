using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class WeatherData
{
    [JsonProperty("weather")]
    public Weather[] Weather { get; set; }

    [JsonProperty("main")]
    public Main Main { get; set; }

    [JsonProperty("list")]
    public List[] List { get; set; }
}

public class Weather
{
    [JsonProperty("description")]
    public string Description { get; set; }
}

public class List
{
    [JsonProperty("weather")]
    public Weather[] Weather { get; set; }

    [JsonProperty("main")]
    public Main Main { get; set; }

    [JsonProperty("dt_txt")]
    public string DateText { get; set; }
}
public class Main
{
    [JsonProperty("temp")]
    public double Temperature { get; set; }

    [JsonProperty("feels_like")]
    public double FeelsLike { get; set; }

    [JsonProperty("humidity")]
    public int Humidity { get; set; }
    
    [JsonProperty("temp_min")]
    public double TempMin { get; set; }

    [JsonProperty("temp_max")]
    public double TempMax { get; set; }

    [JsonProperty("pressure")]
    public int Pressure { get; set; }

}

public class WeatherService1
{
    private const string ApiKey = "eb28ec8266e11659605b5fb4932b4d01";
    private const string BaseUrl = "http://api.openweathermap.org/data/2.5/forecast";

    public async Task<WeatherData> GetWeatherDataAsync(string city)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync($"{BaseUrl}?q={city}&appid={ApiKey}&units=metric&lang=pl");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WeatherData>(json);
            }
            else
            {
                throw new Exception("jest problem…..");
            }
        }
    }
}
public class WeatherService2
{
    private const string ApiKey = "eb28ec8266e11659605b5fb4932b4d01";
    private const string BaseUrl = "http://api.openweathermap.org/data/2.5/weather";

    public async Task<WeatherData> GetWeatherDataAsync(string city)
    {

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync($"{BaseUrl}?q={city}&appid={ApiKey}&units=metric&lang=pl");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WeatherData>(json);
            }
            else
            {
                throw new Exception("jest problem…..");
            }
        }

    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Witamy w WeatherCatchUp");

            string city;
            do
            {
                Console.WriteLine("\nWprowadź nazwę miasta:");
                city = Console.ReadLine();

                
                if (!IsValidCityName(city))
                {
                    Console.WriteLine("\nNazwa miasta może zawierać tylko litery. Spróbuj ponownie.");
                    continue;
                }


                var weatherService1 = new WeatherService1();
                var weatherService2 = new WeatherService2();
                try
                {
                    var weatherData1 = await weatherService1.GetWeatherDataAsync(city);
                    var weatherData2 = await weatherService2.GetWeatherDataAsync(city);

                    Console.WriteLine($"\nTemperatura: {weatherData2.Main.Temperature}°C");
                    Console.WriteLine($"Odczuwalna temperatura: {weatherData2.Main.FeelsLike}°C");
                    Console.WriteLine($"Wilgotność: {weatherData2.Main.Humidity}%");
                    Console.WriteLine($"Opis pogody: {weatherData2.Weather[0].Description}");
                    Console.WriteLine($"Maksymalna temperatura: {weatherData2.Main.TempMax}°C");
                    Console.WriteLine($"Minimalna temperatura: {weatherData2.Main.TempMin}°C");
                    Console.WriteLine($"Ciśnienie: {weatherData2.Main.Pressure}hPa");

                    
                    Console.WriteLine("\nPrognoza pogody na kolejne 10 godziny:");
                    var forecastForNextHours = weatherData1.List
                        .Where(f => DateTime.Parse(f.DateText) - DateTime.Now <= TimeSpan.FromHours(10))
                        .OrderBy(f => DateTime.Parse(f.DateText))
                        .Take(10);

                    foreach (var forecast in forecastForNextHours)
                    {
                        Console.WriteLine($"{forecast.DateText}: Temperatura: {forecast.Main.Temperature}°C, Wilgotność: {forecast.Main.Humidity}%, Opis: {forecast.Weather[0].Description}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd: {ex.Message}. Spróbuj ponownie.");
                }

            } while (string.IsNullOrEmpty(city) || !IsValidCityName(city));
        }

        
        private static bool IsValidCityName(string name)
        {
            return name.All(char.IsLetter);
        }
    }
}
