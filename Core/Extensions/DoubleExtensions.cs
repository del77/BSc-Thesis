using System;

namespace Core.Extensions
{
    public static class DoubleExtensions
    {
        public static int RoundToClosest10(this double value)
        {
            return (int)(Math.Round(value / 10) * 10);
        }
    }
}