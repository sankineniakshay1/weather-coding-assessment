using System.Text.Json;

namespace WeatherExercise.Services;

public sealed class WeatherStore
{
    private readonly string _folder;

    public WeatherStore(IWebHostEnvironment env)
    {
        _folder = Path.Combine(env.ContentRootPath, "weather-data");
        Directory.CreateDirectory(_folder);
    }

    public string PathFor(DateOnly date) => Path.Combine(_folder, $"{date:yyyy-MM-dd}.json");

    public bool Exists(DateOnly date) => File.Exists(PathFor(date));

    public async Task WriteRawJsonAsync(DateOnly date, string json, CancellationToken ct)
        => await File.WriteAllTextAsync(PathFor(date), json, ct);

    public async Task<string?> ReadRawJsonAsync(DateOnly date, CancellationToken ct)
    {
        var path = PathFor(date);
        return File.Exists(path) ? await File.ReadAllTextAsync(path, ct) : null;
    }

    public static string Serialize<T>(T value)
        => JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
}
