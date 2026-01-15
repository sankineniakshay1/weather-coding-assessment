using System.Text.Json.Serialization;

namespace WeatherExercise.Models;

public sealed class OpenMeteoArchiveResponse
{
    [JsonPropertyName("daily")]
    public DailyBlock? Daily { get; set; }

    public sealed class DailyBlock
    {
        [JsonPropertyName("time")]
        public List<string>? Time { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public List<double>? TempMin { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public List<double>? TempMax { get; set; }

        [JsonPropertyName("precipitation_sum")]
        public List<double>? PrecipSum { get; set; }
    }
}
