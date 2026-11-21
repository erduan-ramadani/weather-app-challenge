using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherApi.Repositories;
using WeatherApi.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("WeatherDb"));

builder.Services.AddScoped<ICityRepository, CityRepository>();

builder.Services.AddScoped<CityService>();

builder.Services.AddHttpClient<IOpenMeteoClient, OpenMeteoClient>(
    client =>
    {
        client.Timeout = TimeSpan.FromSeconds(10);
    });

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();