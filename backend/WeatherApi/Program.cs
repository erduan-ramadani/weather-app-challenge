using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherApi.Repositories;
using WeatherApi.Clients;
using WeatherApi.Services;
using WeatherApi.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("WeatherDb"));

builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();

builder.Services.AddScoped<CityService>();
builder.Services.AddScoped<WeatherService>();

builder.Services.AddHttpClient<IOpenMeteoClient, OpenMeteoClient>(
    client =>
    {
        client.Timeout = TimeSpan.FromSeconds(10);
    });

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
