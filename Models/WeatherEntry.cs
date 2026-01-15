namespace WeatherExercise.Models;

public sealed record WeatherEntry
{
    public string Input { get; init; } = "";
    public string? Date { get; init; } // yyyy-MM-dd if valid

    public double? MinTempC { get; init; }
    public double? MaxTempC { get; init; }
    public double? PrecipMm { get; init; }

    // OK | Cached | InvalidDate | ApiError | NoData
    public string Status { get; init; } = "OK";
    public string? Message { get; init; }
}
