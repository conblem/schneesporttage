using System.Diagnostics;
using api.Entities;
using api.Repos;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ActivitySource _activitySource;

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IUserRepo _userRepo;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        IUserRepo userRepo)
    {
        _logger = logger;
        _activitySource = new ActivitySource("ml.schneesporttage.api.development");
        _userRepo = userRepo;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        using var activity = _activitySource.StartActivity("Test");
        activity?.SetTag("tag", "value");

        var user = new User { Firstname = "Conblem", Lastname = "Test" };
        await _userRepo.Insert(user);

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}