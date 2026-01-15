using System.Text.Json;
using WeatherExercise.Models;

namespace WeatherExercise.Services;

public sealed class WeatherAggregator
{
    private readonly OpenMeteoClient _client;
    private readonly WeatherStore _store;
    private readonly IWebHostEnvironment _env;

    public WeatherAggregator(OpenMeteoClient client, WeatherStore store, IWebHostEnvironment env)
    {
        _client = client;
        _store = store;
        _env = env;
    }

    public async Task<List<WeatherEntry>> LoadAsync(CancellationToken ct)
    {
        var datesPath = Path.Combine(_env.ContentRootPath, "dates.txt");
        var lines = File.Exists(datesPath)
            ? await File.ReadAllLinesAsync(datesPath, ct)
            : Array.Empty<string>();

        var results = new List<WeatherEntry>();

        foreach (var line in lines)
        {
            var input = (line ?? string.Empty).Trim();
            var (ok, date, error) = DateParser.TryParse(input);

            if (!ok || date is null)
            {
                results.Add(new WeatherEntry
                {
                    Input = input,
                    Status = "InvalidDate",
                    Message = error
                });
                continue;
            }

            if (_store.Exists(date.Value))
            {
                var raw = await _store.ReadRawJsonAsync(date.Value, ct);
                if (TryMapFromRaw(input, date.Value, raw, out var cached))
                {
                    results.Add(cached with { Status = "Cached" });
                }
                else
                {
                    results.Add(new WeatherEntry
                    {
                        Input = input,
                        Date = date.Value.ToString("yyyy-MM-dd"),
                        Status = "NoData",
                        Message = "Cached JSON exists but could not be parsed"
                    });
                }

                continue;
            }

            try
            {
                var api = await _client.GetDailyAsync(date.Value, ct);

                // Store raw response
                var rawJson = WeatherStore.Serialize(api);
                await _store.WriteRawJsonAsync(date.Value, rawJson, ct);

                if (TryMapFromApi(input, date.Value, api, out var mapped))
                {
                    results.Add(mapped);
                }
                else
                {
                    results.Add(new WeatherEntry
                    {
                        Input = input,
                        Date = date.Value.ToString("yyyy-MM-dd"),
                        Status = "NoData",
                        Message = "API returned empty or missing daily data"
                    });
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                results.Add(new WeatherEntry
                {
                    Input = input,
                    Date = date.Value.ToString("yyyy-MM-dd"),
                    Status = "ApiError",
                    Message = ex.Message
                });
            }
        }

        return results;
    }

    private static bool TryMapFromApi(string input, DateOnly date, OpenMeteoArchiveResponse api, out WeatherEntry entry)
    {
        entry = new WeatherEntry
        {
            Input = input,
            Date = date.ToString("yyyy-MM-dd"),
            Status = "OK"
        };

        var d = api.Daily;
        if (d?.Time is null || d.Time.Count == 0) return false;
        if (d.TempMin is null || d.TempMax is null || d.PrecipSum is null) return false;
        if (d.TempMin.Count < 1 || d.TempMax.Count < 1 || d.PrecipSum.Count < 1) return false;

        entry = entry with
        {
            MinTempC = d.TempMin[0],
            MaxTempC = d.TempMax[0],
            PrecipMm = d.PrecipSum[0]
        };

        return true;
    }

    private static bool TryMapFromRaw(string input, DateOnly date, string? rawJson, out WeatherEntry entry)
    {
        entry = new WeatherEntry { Input = input, Date = date.ToString("yyyy-MM-dd") };

        if (string.IsNullOrWhiteSpace(rawJson)) return false;

        try
        {
            var api = JsonSerializer.Deserialize<OpenMeteoArchiveResponse>(rawJson);
            return api is not null && TryMapFromApi(input, date, api, out entry);
        }
        catch
        {
            return false;
        }
    }
}
