
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsProjectMVC;
using NewsProjectMVC.Models.Db;
using NewsProjectMVC.Models.ViewModels;
using System.Text.Json.Nodes;

public class WeatherViewComponent : ViewComponent
{
    private readonly MyNewsContext _context;

    public WeatherViewComponent(MyNewsContext context)
    {
        _context = context;
    }

    // GET: MENUS
    public async Task<IViewComponentResult> InvokeAsync()
    {

        const string apiUrl = WeatherAPI.weather_url;

        using (var httpClient = new HttpClient())
        {
            try
            {
                string? jsonResponse = await httpClient.GetStringAsync(apiUrl);
                var weatherData = JsonNode.Parse(jsonResponse);

                var timeArray = weatherData["hourly"]["time"].AsArray();
                var tempArray = weatherData["hourly"]["temperature_2m"].AsArray();

               
                var currentDateHourString = DateTime.Now.ToString("yyyy-MM-dd'T'HH:00");

                var timeList = timeArray.Select(timeItem => timeItem.GetValue<string>()).ToList();
                int currentIndex = timeList.FindIndex(time => time == currentDateHourString);

                int currentTemp = 0;
                if (currentIndex != -1)
                {
                    currentTemp = (int)Math.Round(tempArray[currentIndex].GetValue<double>());
                }

                var viewModel = new WeatherViewModel()
                {
                    Temperature = currentTemp,
                    City = "LONDON",
                    CurrentDate = DateTime.Now
                };
                 
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // If any error occurs (e.g., API is down), log the exception for debugging.
                Console.WriteLine($"Error fetching weather data: {ex.Message}");

                // Return the view with default error data.
                var errorViewModel = new WeatherViewModel { Temperature = 0, City = "Error", CurrentDate = DateTime.Now };
                return View(errorViewModel);
            }
        }
    }


}
