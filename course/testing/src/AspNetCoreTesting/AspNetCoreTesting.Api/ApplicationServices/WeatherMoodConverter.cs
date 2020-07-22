namespace AspNetCoreTesting.Api.ApplicationServices
{
    public class WeatherMoodConverter
    {
        public string WeatherToMood(WeatherForecast weather) => weather switch
        {

            WeatherForecast f when f.TemperatureC < -50 => "🥶",
            WeatherForecast f when f.TemperatureC < 0 => "😨",
            WeatherForecast f when f.TemperatureC > 20 && f.Summary == "Sunny" => "😍",
            WeatherForecast f when f.TemperatureC > 30 => "🥵",
            _ => "😐"
        };
    }
}
