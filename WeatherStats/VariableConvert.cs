using System.Numerics;

namespace WeatherStats
{
    /// <summary>
    /// Utility methods for converting between meteorological units and derived quantities.
    /// </summary>
    public static class VariableConvert
    {
        private const float beta = 17.625f;
        private const float lambda = 243.04f;

        /// <summary>
        /// Calculates relative humidity from dew point and air temperature using the August-Roche-Magnus approximation.
        /// </summary>
        /// <param name="dewpointInCelcius">Dew point temperature in °C.</param>
        /// <param name="temperatureInCelcius">Air temperature in °C.</param>
        /// <returns>Relative humidity in percent, clamped to [0, 100].</returns>
        public static float RelativeHumidity(float dewpointInCelcius, float temperatureInCelcius)
        {
            return MathF.Max(MathF.Min(100 * MathF.Exp(beta * dewpointInCelcius / (lambda + dewpointInCelcius)) / MathF.Exp(beta * temperatureInCelcius / (lambda + temperatureInCelcius)), 100), 0);
        }

        /// <summary>
        /// Converts a temperature from Kelvin to Celsius.
        /// </summary>
        /// <param name="valueInKelvin">Temperature in Kelvin.</param>
        /// <returns>Temperature in °C.</returns>
        public static float KelvinToCelcius(float valueInKelvin)
        {
            return valueInKelvin - 273.15f;
        }

        private static readonly WindDirection8[] Directions8 = new[]
        {
            WindDirection8.West, // -4
            WindDirection8.SouthWest, // -3
            WindDirection8.South, // -2
            WindDirection8.SouthEast, // -1
            WindDirection8.East, // 0
            WindDirection8.NorthEast, // 1
            WindDirection8.North, // 2
            WindDirection8.NorthWest, // 3
            WindDirection8.West // 4
        };

        /// <summary>
        /// Determines the 8-point compass direction of a wind vector.
        /// </summary>
        /// <param name="windSpeed">Wind velocity vector where X is the eastward component and Y is the northward component.</param>
        /// <returns>The closest <see cref="WindDirection8"/> to the vector's bearing.</returns>
        public static WindDirection8 GetWindDirection8(Vector2 windSpeed)
        {
            return Directions8[(int)MathF.Round(MathF.Atan2(windSpeed.Y, windSpeed.X) * 4 / MathF.PI) + 4];
        }
    }
}
