# Pmad.WeatherStats

[![NuGet](https://img.shields.io/nuget/v/Pmad.WeatherStats)](https://www.nuget.org/packages/Pmad.WeatherStats)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](../LICENSE)

A .NET 8 solution for generating and querying ERA5 average weather statistics (temperature, humidity, wind speed, wind direction) for any geographic point on Earth.

The public database is hosted at **https://weatherdata.pmad.net/ERA5AVG/**. It provides averages for 2010-2020.

---

## Projects

| Project | Description |
|---|---|
| [`Pmad.WeatherStats`](WeatherStats/README.md) | Class library for querying the ERA5 average weather statistics database |
| `Pmad.WeatherStatsGenerator` | CLI tool for processing raw ERA5 HDF5 files and generating the database |
| `Pmad.WeatherStats.Test` | Unit and integration tests for the `Pmad.WeatherStats` library |

---

## Getting started

See the **[Pmad.WeatherStats library documentation](WeatherStats/README.md)** for usage instructions and code examples.
