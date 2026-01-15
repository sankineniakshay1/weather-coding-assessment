using System.Globalization;

namespace WeatherExercise.Services;

public static class DateParser
{
    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US");

    private static readonly string[] ExactFormats =
    {
        "MM/dd/yyyy",
        "MMMM d, yyyy",
        "MMMM dd, yyyy",
        "MMM-dd-yyyy",
        "MMM-d-yyyy",
        "MMMM d yyyy"
    };

    public static (bool ok, DateOnly? date, string? error) TryParse(string input)
    {
        var trimmed = (input ?? string.Empty).Trim();
        if (trimmed.Length == 0) return (false, null, "Empty line");

        if (DateTime.TryParseExact(
                trimmed,
                ExactFormats,
                UsCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                out var dtExact))
        {
            return (true, DateOnly.FromDateTime(dtExact), null);
        }

        if (DateTime.TryParse(
                trimmed,
                UsCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                out var dt))
        {
            return (true, DateOnly.FromDateTime(dt), null);
        }

        return (false, null, "Invalid date format or non-existent date");
    }
}
