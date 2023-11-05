using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("default", (httpClient) =>
{
    httpClient.BaseAddress = new Uri("http://172.17.0.3/");
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

if (app.Environment.IsProduction())
{
    app.MapGet("/search", async (CancellationToken token, IHttpClientFactory httpClientFactory) =>
    {
        var httpResponseMessage = await httpClientFactory.CreateClient("default").GetAsync("weatherforecast");

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return await httpResponseMessage.Content.ReadAsStringAsync(token);
        }

        return "Nothing found";
    }).WithName("Search").WithOpenApi();
}

app.MapGet("item/{id:int:range(1,394958)}", Results<Ok<string>, BadRequest> (int id) =>
{
    return TypedResults.Ok($"Item {id} found");
}).AddEndpointFilter(async (invocationContext, next) =>
{
    var id = invocationContext.GetArgument<int>(0);
    if (id == default)
    {
        return TypedResults.BadRequest();
    }
    return await next(invocationContext);
}).WithName("GetItem").WithOpenApi(generatedOperation =>
{
    return generatedOperation;
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
