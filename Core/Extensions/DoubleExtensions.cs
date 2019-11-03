using System;
using System.Globalization;

namespace Core.Extensions
{
    public static class DoubleExtensions
    {
        public static int RoundToClosest10(this double value)
        {
            return (int)(Math.Round(value / 10) * 10);
        }

        public static string ToStringWithDot(this double value)
        {
            return value.ToString(new CultureInfo("en-US"));

            return value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}