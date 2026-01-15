# WeatherExercise (Open-Meteo Historical Weather)

This repo implements the coding exercise requirements:
- Read and parse `dates.txt` (multiple formats)
- Handle invalid dates gracefully (no crash)
- Call Open-Meteo Historical Weather API for Dallas, TX
- Cache/store per-date JSON under `weather-data/`
- Expose `GET /api/weather`
- Render results in a simple UI (Razor Pages) with loading/error states and a small interaction

## Prerequisites
- .NET 6+ SDK (tested target: net8.0)

## Run locally

```bash
dotnet restore
dotnet run
```

Then open the URL shown in the console (typically `http://localhost:5000`).

## API
- `GET /api/weather` returns one entry per input line, including:
  - normalized date (`yyyy-MM-dd`) when valid
  - min temp (°C), max temp (°C), precipitation sum (mm) when available
  - status and error message for invalid dates or failures

## UI
- Home page (`/`) calls `/api/weather` and displays results in a table.
- Includes:
  - loading state
  - error message on failure
  - interactions: sort buttons + min-temperature filter + click row to show details

## Assumptions
- Location is fixed to Dallas, TX (32.78, -96.8) as specified in the prompt.
- If `weather-data/yyyy-MM-dd.json` exists, the app will not re-call the API for that date.
- Invalid dates (example: `April 31, 2022`) are returned with `Status=InvalidDate`.
