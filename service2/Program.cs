using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () =>
{
    if (new Random().Next(0, 2) == 1)
    {
        return Results.Problem("bad luck", statusCode: StatusCodes.Status429TooManyRequests);
    }

    return Results.Ok("Sunny");
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
