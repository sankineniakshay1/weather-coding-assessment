using System.Net.Http.Json;
using WeatherExercise.Models;

namespace WeatherExercise.Services;

public sealed class OpenMeteoClient
{
    private readonly HttpClient _http;

    // Dallas, TX approximate coordinates per prompt
    private const double Lat = 32.78;
    private const double Lon = -96.8;

    public OpenMeteoClient(HttpClient http) => _http = http;

    public async Task<OpenMeteoArchiveResponse> GetDailyAsync(DateOnly date, CancellationToken ct)
    {
        var iso = date.ToString("yyyy-MM-dd");

        var url =
            "https://archive-api.open-meteo.com/v1/archive" +
            $"?latitude={Lat}&longitude={Lon}" +
            $"&start_date={iso}&end_date={iso}" +
            "&daily=temperature_2m_max,temperature_2m_min,precipitation_sum" +
            "&timezone=auto";

        using var resp = await _http.GetAsync(url, ct);
        resp.EnsureSuccessStatusCode();

        var data = await resp.Content.ReadFromJsonAsync<OpenMeteoArchiveResponse>(cancellationToken: ct);
        return data ?? new OpenMeteoArchiveResponse();
    }
}
