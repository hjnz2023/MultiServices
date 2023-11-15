using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("default", (httpClient) =>
{
    var service2Url = Environment.GetEnvironmentVariable("SERVICE2_URL") ?? "http://service2/";
    httpClient.BaseAddress = new Uri(service2Url);
});
// builder.Services.Configure<RouteHandlerOptions>(options =>
// {
//     options.ThrowOnBadRequest = false;
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.RegisterWeatherForecastEndpoints();
app.RegisterTestEndpoints();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
