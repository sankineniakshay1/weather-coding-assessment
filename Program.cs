using WeatherExercise.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddHttpClient<OpenMeteoClient>(http =>
{
    http.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddSingleton<WeatherStore>();
builder.Services.AddScoped<WeatherAggregator>();

var app = builder.Build();

app.UseStaticFiles();
app.MapRazorPages();

app.MapGet("/api/weather", async (WeatherAggregator agg, CancellationToken ct) =>
{
    var data = await agg.LoadAsync(ct);
    return Results.Ok(data);
});

app.Run();
