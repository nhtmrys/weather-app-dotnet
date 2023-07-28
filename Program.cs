using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


class Program
{
    const string ApiUrl = "https://api.openweathermap.org/data/2.5/weather";
    const string ApiKey = "1bf3015a88049c647d0e120ad794b0c6"; // OpenWeatherMap API anahtarınızı buraya ekleyin

    static async Task Main(string[] args)
    {
        while (true)
        {
            Console.Write("Lütfen hava durumunu öğrenmek istediğiniz şehir adını girin: ");
            string? city = Console.ReadLine();

            var weatherData = await GetWeatherDataAsync(city);
            if (!string.IsNullOrEmpty(weatherData))
            {
                string weatherDescription = ParseWeatherDescriptionFromResponse(weatherData);
                string weatherIcon = ParseWeatherIconFromResponse(weatherData);
                Console.WriteLine($"Hava durumu açıklaması: {weatherDescription} {weatherIcon}");
            }
        }
    }

    static async Task<string> GetWeatherDataAsync(string? city)
    {
        using var client = new HttpClient();

        try
        {
            var response = await client.GetAsync($"{ApiUrl}?q={city}&appid={ApiKey}&units=metric");
            if (response != null && response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
            }


            string? responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Hava durumu verileri alınamadı: " + ex.Message);
            return string.Empty;
        }
    }
    static string? ParseWeatherDescriptionFromResponse(string response)
    {
        JObject jObject = JObject.Parse(response);
        string weatherDescription = (string)jObject["weather"][0]["description"];
        return weatherDescription;
    }
    static string? ParseWeatherIconFromResponse(string response)
    {
        JObject jObject = JObject.Parse(response);
        string? weatherDescription = (string)jObject["weather"][0]["description"];

        // Hava durumu sembollerini temsil eden bir Dictionary oluşturuyoruz
        Dictionary<string, string> weatherIcons = new Dictionary<string, string>
    {
        { "clear sky", "☀️" },
        { "few clouds", "🌤️" },
        { "scattered clouds", "⛅" },
        {"overcast clouds", "☁️"},
        { "broken clouds", "☁️" },
        { "shower rain", "🌧️" },
        { "rain", "🌧️" },
        { "thunderstorm", "⛈️" },
        { "snow", "🌨️" },
        { "mist", "🌫️" },

    };
                 // Eğer sembol bulunamazsa varsayılan sembolü kullanılır.
        if (weatherIcons.TryGetValue(weatherDescription?.ToLower(), out string weatherIcon))
        {
            return weatherIcon;
        }
        else
        {
            return "🌈"; // Varsayılan sembol
        }
    }
}
