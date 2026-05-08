# Pmad.WeatherStats

[![NuGet](https://img.shields.io/nuget/v/Pmad.WeatherStats)](https://www.nuget.org/packages/Pmad.WeatherStats)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](../LICENSE)

A .NET 8 library for reading ERA5 average weather statistics from a local directory or a remote HTTP database. 

Raw data need to be converted using the `Pmad.WeatherStatsGenerator` tool (see the [main README](README.md) for details). 

The public database is hosted at **https://weatherdata.pmad.net/ERA5AVG/**. It provides averages for 2010-2020.

---

## Installation

Add the `Pmad.WeatherStats` package to your project.

---

## Quick start

### 1. Open a database

```csharp
using Pmad.WeatherStats.Databases;

// From the public HTTP database
var db = WeatherStatsDatabase.Create("https://weatherdata.pmad.net/ERA5AVG/");

// Or from a local directory that contains the JSON data files
var db = WeatherStatsDatabase.Create("/path/to/era5avg/");
```

### 2. Query a geographic point

Pass any latitude/longitude pair. The library automatically snaps to the nearest
0.25° ERA5 grid point. Negative longitudes (West) are normalised automatically.

```csharp
// Geneva, Switzerland
var stats = await db.GetStats(46.2, 6.15);

if (stats == null)
{
    Console.WriteLine("No data available for this point (e.g. open ocean).");
    return;
}

Console.WriteLine($"Grid point: {stats.Latitude}°N {stats.Longitude}°E");
Console.WriteLine($"Data covers {stats.Months.Length} months");
```

### 3. Read monthly statistics

`stats.Months` is a 12-element array ordered **January (index 0) through December (index 11)**.

```csharp
string[] monthNames = [
    "January","February","March","April","May","June",
    "July","August","September","October","November","December"
];

for (int i = 0; i < 12; i++)
{
    var month = stats.Months[i];

    Console.WriteLine($"--- {monthNames[i]} ---");

    // Temperature (°C)
    Console.WriteLine($"  Temperature  min {month.Temperature.Min.Min:F1} °C " +
                      $"/ avg {month.Temperature.Avg.Avg:F1} °C " +
                      $"/ max {month.Temperature.Max.Max:F1} °C");

    // Relative humidity (%)
    Console.WriteLine($"  Humidity     avg {month.Humidity.Avg:F1} %");

    // Wind speed (m/s)
    Console.WriteLine($"  Wind speed   avg {month.WindSpeed.Avg.Avg:F1} m/s");

    // Prevailing wind direction
    Console.WriteLine($"  Wind dir     {month.WindDirection.Prevailing}");
}
```

---

## Data model

### `YearWeatherStatsPoint`

| Property | Type | Description |
|---|---|---|
| `Latitude` | `float` | Grid point latitude (°N) |
| `Longitude` | `float` | Grid point longitude (°E, 0–360) |
| `Months` | `MonthWeatherStatsData[12]` | Monthly statistics (Jan–Dec) |

### `MonthWeatherStatsData`

| Property | Type | Description |
|---|---|---|
| `Temperature` | `MinMaxAvgStats` | Air temperature (°C) |
| `Humidity` | `MinMaxAvg` | Relative humidity (%) |
| `WindSpeed` | `MinMaxAvgStats` | Wind speed (m/s) |
| `WindDirection` | `WindDirectionStats` | Wind direction statistics |

### `MinMaxAvg`

Holds the **minimum**, **average**, and **maximum** of a set of values.

| Property | Description |
|---|---|
| `Min` | Minimum value |
| `Avg` | Average value |
| `Max` | Maximum value |

### `MinMaxAvgStats`

Aggregates daily `MinMaxAvg` slices into overall min/avg/max statistics.

| Property | Type | Description |
|---|---|---|
| `Min` | `MinMaxAvg` | Statistics of daily minimum values |
| `Avg` | `MinMaxAvg` | Statistics of daily average values |
| `Max` | `MinMaxAvg` | Statistics of daily maximum values |

### `WindDirectionStats`

| Property / Method | Description |
|---|---|
| `Prevailing` | Most frequent `WindDirection8` direction |
| `GetProbability(dir)` | Occurrence probability for the given direction (0–1) |
| `GetAverageSpeed(dir)` | Average speed (m/s) for the given direction |
| `Probability[]` | Raw probability array indexed by `WindDirection8` |
| `AverageSpeed[]` | Raw speed array indexed by `WindDirection8` |

### `WindDirection8`

Eight-point compass enum. Each value represents the direction the wind is blowing **toward**.

`North`, `NorthEast`, `East`, `SouthEast`, `South`, `SouthWest`, `West`, `NorthWest`

---

## Utility methods

### `VariableConvert`

```csharp
using Pmad.WeatherStats;

// Relative humidity from dew point and air temperature (August-Roche-Magnus approximation)
float rh = VariableConvert.RelativeHumidity(dewpointCelsius, temperatureCelsius);

// Convert Kelvin to Celsius
float tempC = VariableConvert.KelvinToCelcius(tempKelvin);

// Determine wind direction from a velocity vector (X = east, Y = north)
using System.Numerics;
WindDirection8 dir = VariableConvert.GetWindDirection8(new Vector2(eastward, northward));
```

---

## Advanced: custom storage back-end

Implement `IWeatherStatsStorage` to load data from any source (cloud blob storage, embedded resources, etc.):

```csharp
using Pmad.WeatherStats.Databases;
using Pmad.WeatherStats.Stats;

public class MyStorage : IWeatherStatsStorage
{
    public async Task<List<YearWeatherStatsPoint>> Load(string path)
    {
        // path is a filename such as "ERA5AVG_N46_006.json"
        // return an empty list when the file does not exist
        ...
    }
}

var db = new WeatherStatsDatabase(new MyStorage());
```

---

## Advanced: cell file naming

The database shards data into 2°×2° cell files. You can resolve the file name for any grid point:

```csharp
var index = WeatherStatsDatabase.GetCellIndex(46.25f, 6.25f);   // (46, 6)
var file  = WeatherStatsDatabase.GetCellFileName(index);         // "ERA5AVG_N46_006.json"
```
