# weatherstats

A .NET 8 solution for generating and querying ERA5 average weather statistics (temperature, humidity, wind speed, wind direction) for any geographic point on Earth.

The public database is hosted at **https://weatherdata.pmad.net/ERA5AVG/**. It provides averages for 2010-2020.

---

## Projects

| Project | Description |
|---|---|
| [`WeatherStats`](WeatherStats/README.md) | Class library for querying the ERA5 average weather statistics database |
| `WeatherStatsGenerator` | CLI tool for processing raw ERA5 HDF5 files and generating the database |
| `WeatherStats.Test` | Unit and integration tests for the `WeatherStats` library |

---

## Getting started

See the **[WeatherStats library documentation](WeatherStats/README.md)** for usage instructions and code examples.
