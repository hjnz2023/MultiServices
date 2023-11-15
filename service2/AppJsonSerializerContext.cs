using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

[JsonSerializable(typeof(WeatherForecast)), JsonSerializable(typeof(ProblemDetails))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
