using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class WeatherData
{
    [JsonProperty("weather")]
    public Weather[] Weather { get; set; }

    [JsonProperty("main")]
    public Main Main { get; set; }

    [JsonProperty("list")]
    public List[] List { get; set; }

    [JsonProperty("timezone")]
    public int Timezone { get; set; } 

    [JsonProperty("name")]
    public string CityName { get; set; }  
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

public class WeatherService
{
    private const string ApiKey = "eb28ec8266e11659605b5fb4932b4d01";
    private const string BaseUrlForecast = "http://api.openweathermap.org/data/2.5/forecast";
    private const string BaseUrlWeather = "http://api.openweathermap.org/data/2.5/weather";

    public async Task<WeatherData> GetWeatherDataByCityAsync(string city)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync($"{BaseUrlWeather}?q={city}&appid={ApiKey}&units=metric&lang=pl");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WeatherData>(json);
            }
            else
            {
                throw new Exception("sprawdz czy dobrze wpisano miasto. jesli tak to jest problem....");
            }
        }
    }

    public async Task<WeatherData> GetWeatherDataByCoordinatesAsync(double latitude, double longitude)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync($"{BaseUrlWeather}?lat={latitude}&lon={longitude}&appid={ApiKey}&units=metric&lang=pl");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WeatherData>(json);
            }
            else
            {
                throw new Exception("sprawdz czy dobrze wpisano współrzędne. jesli tak to jest problem....");
            }
        }
    }

    public async Task<WeatherData> GetWeatherForecastByCityAsync(string city)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync($"{BaseUrlForecast}?q={city}&appid={ApiKey}&units=metric&lang=pl");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WeatherData>(json);
            }
            else
            {
                throw new Exception("sprawdz czy dobrze wpisano miasto. jesli tak to jest problem....");
            }
        }
    }

    public async Task<WeatherData> GetWeatherForecastByCoordinatesAsync(double latitude, double longitude)
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync($"{BaseUrlForecast}?lat={latitude}&lon={longitude}&appid={ApiKey}&units=metric&lang=pl");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<WeatherData>(json);
            }
            else
            {
                throw new Exception("sprawdz czy dobrze wpisano współrzędne. jesli tak to jest problem....");
            }
        }
    }
}

class Program
{
    static WeatherService weatherService = new WeatherService();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Witamy w WeatherCatchUp");

        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Wyszukaj pogodę dla miasta");
            Console.WriteLine("2. Wyszukaj pogodę dla szerokości i długości geograficznej");
            Console.WriteLine("3. Wyjdź");
            Console.Write("\nWybierz opcję: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await HandleCityWeather();
                    break;
                case "2":
                    await HandleCoordinatesWeather();
                    break;
                case "3":
                    Console.WriteLine("Do zobaczenia!");
                    return;
                default:
                    Console.WriteLine("Niepoprawny wybór. Spróbuj ponownie.");
                    break;
            }
        }
    }

    private static async Task HandleCityWeather()
    {
        while (true)
        {
            Console.Write("\nWprowadź nazwę miasta: ");
            string city = Console.ReadLine();
            if (IsValidCityName(city))
            {
                try
                {
                    var currentWeather = await weatherService.GetWeatherDataByCityAsync(city);
                    var forecastWeather = await weatherService.GetWeatherForecastByCityAsync(city);
                    DisplayWeather(currentWeather, forecastWeather);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd: {ex.Message}. Spróbuj ponownie.");
                }
            }
            else
            {
                Console.WriteLine("Nazwa miasta może zawierać tylko litery i spacje. Spróbuj ponownie.");
            }

            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować, lub 'q' aby wrócić do menu głównego.");
            if (Console.ReadKey().Key == ConsoleKey.Q)
                break;
        }
    }

    private static async Task HandleCoordinatesWeather()
    {
        while (true)
        {
            Console.Write("\nWprowadź szerokość geograficzną: ");
            string latitudeInput = Console.ReadLine();
            Console.Write("Wprowadź długość geograficzną: ");
            string longitudeInput = Console.ReadLine();

            if (double.TryParse(latitudeInput, out double latitude) && double.TryParse(longitudeInput, out double longitude))
            {
                try
                {
                    var currentWeather = await weatherService.GetWeatherDataByCoordinatesAsync(latitude, longitude);
                    var forecastWeather = await weatherService.GetWeatherForecastByCoordinatesAsync(latitude, longitude);
                    DisplayWeather(currentWeather, forecastWeather);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd: {ex.Message}. Spróbuj ponownie.");
                }
            }
            else
            {
                Console.WriteLine("Niepoprawne współrzędne. Spróbuj ponownie.");
            }

            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować, lub 'q' aby wrócić do menu głównego.");
            if (Console.ReadKey().Key == ConsoleKey.Q)
                break;
        }
    }

    private static void DisplayWeather(WeatherData currentWeather, WeatherData forecastWeather)
    {
        Console.WriteLine($"\nAktualna pogoda dla: {currentWeather.CityName}");
        DisplayWeatherImage(currentWeather.Weather[0].Description);
        Console.WriteLine($"Temperatura: {currentWeather.Main.Temperature}°C");
        Console.WriteLine($"Odczuwalna temperatura: {currentWeather.Main.FeelsLike}°C");
        Console.WriteLine($"Wilgotność: {currentWeather.Main.Humidity}%");
        Console.WriteLine($"Opis pogody: {currentWeather.Weather[0].Description}");
        Console.WriteLine($"Maksymalna temperatura: {currentWeather.Main.TempMax}°C");
        Console.WriteLine($"Minimalna temperatura: {currentWeather.Main.TempMin}°C");
        Console.WriteLine($"Ciśnienie: {currentWeather.Main.Pressure}hPa");

        // Wyświetlanie strefy czasowej
        Console.WriteLine($"Strefa czasowa: UTC{currentWeather.Timezone / 3600:+#;-#}");

        // Wyświetlanie prognozy pogody
        Console.WriteLine("\nPrognoza pogody na kolejne godziny:");
        var forecastForNextHours = forecastWeather.List
            .Where(f => DateTime.Parse(f.DateText) - DateTime.Now <= TimeSpan.FromHours(10))
            .OrderBy(f => DateTime.Parse(f.DateText))
            .Take(10);
        foreach (var forecast in forecastForNextHours)
        {
            Console.WriteLine($"{forecast.DateText}: Temperatura: {forecast.Main.Temperature}°C, Wilgotność: {forecast.Main.Humidity}%, Opis: {forecast.Weather[0].Description}");
            DisplayWeatherImage(forecast.Weather[0].Description);
        }
    }

    private static void DisplayWeatherImage(string description)
    {
        switch (description.ToLower())
        {
            case "clear sky":
            case "bezchmurnie":
                Console.WriteLine("   \\   /   ");
                Console.WriteLine("    .-.    ");
                Console.WriteLine(" ― (   ) ― ");
                Console.WriteLine("    `-’    ");
                Console.WriteLine("   /   \\   ");
                break;
            case "few clouds":
            case "zachmurzenie duże":
                Console.WriteLine("    .--.   ");
                Console.WriteLine(" .-(    ). ");
                Console.WriteLine("(___.__)__)");
                break;
            case "scattered clouds":
            case "rozproszone chmury":
                Console.WriteLine("    .--.   ");
                Console.WriteLine(" .-(    ). ");
                Console.WriteLine("(___.__)__)");
                break;
            case "cloudy":
            case "pochmurnie":
                Console.WriteLine("    .--.   ");
                Console.WriteLine(" .-(    ). ");
                Console.WriteLine("(___.__)__)");
                break;
            case "broken clouds":
            case "zachmurzenie umiarkowane":
                Console.WriteLine("    .--.   ");
                Console.WriteLine(" .-(    ). ");
                Console.WriteLine("(___.__)__)");
                break;
            case "overcast clouds":
            case "zachmurzenie małe":
                Console.WriteLine("             ");
                Console.WriteLine("             ");
                Console.WriteLine(" (  )___(   ");
                Console.WriteLine("      _   ) ");
                Console.WriteLine("    (_)__)  ");
                break;
            case "shower rain":
            case "przelotne opady":
                Console.WriteLine("     .-.     ");
                Console.WriteLine("    (   ).   ");
                Console.WriteLine("   (___(__)  ");
                Console.WriteLine("   ‚‘‚‘‚‘‚‘  ");
                Console.WriteLine("   ‚’‚’‚’‚’  ");
                break;
            case "low rain":
            case "słabe opady deszczu":
                Console.WriteLine("     .-.     ");
                Console.WriteLine("    (   ).   ");
                Console.WriteLine("   (___(__)  ");
                Console.WriteLine("   ‘‚‘‚‘‚‘   ");
                Console.WriteLine("   ’‚’‚’‚’   ");
                break;
            case "light drizzle":
            case "słaba mżawka":
                Console.WriteLine("     .-.     ");
                Console.WriteLine("    (   ).   ");
                Console.WriteLine("   (___(__)  ");
                Console.WriteLine("   ‚‘‚‘‚‘‚‘  ");
                Console.WriteLine("   ‚’‚’‚’‚’  ");
                break;
            case "rain":
            case "duże opady deszczu":
                Console.WriteLine("     .-.     ");
                Console.WriteLine("    (   ).   ");
                Console.WriteLine("   (___(__)  ");
                Console.WriteLine("   ‚‘‚‘‚‘‚‘  ");
                Console.WriteLine("   ‚’‚’‚’‚’  ");
                break;
            case "moderate rainfall":
            case "umiarkowane opady deszczu":
                Console.WriteLine("     .-.     ");
                Console.WriteLine("    (   ).   ");
                Console.WriteLine("   (___(__)  ");
                Console.WriteLine("   ‚‘‚‘‚‘‚‘  ");
                Console.WriteLine("   ‚’‚’‚’‚’  ");
                break;
            case "thunderstorm":
            case "burza":
                Console.WriteLine("     .-.     ");
                Console.WriteLine("    (   ).   ");
                Console.WriteLine("   (___(__)  ");
                Console.WriteLine("   ‚‘⚡‘‚‘   ");
                Console.WriteLine("   ‚’⚡’‚’   ");
                break;
            case "snow":
            case "śnieg":
                Console.WriteLine("     .-.     ");
                Console.WriteLine("    (   ).   ");
                Console.WriteLine("   (___(__)  ");
                Console.WriteLine("   *  *  *   ");
                Console.WriteLine("  *  *  *  * ");
                break;
            case "mist":
            case "zamglenia":
                Console.WriteLine("     -_-     ");
                Console.WriteLine("    -_-_-    ");
                Console.WriteLine("   -_-_-_-   ");
                Console.WriteLine("  -_-_-_-_-  ");
                break;
            default:

                Console.WriteLine("Brak obrazu dla tej pogody.");
                break;
        }
    }

    private static bool IsValidCityName(string name)
    {
        return name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
    }

}
