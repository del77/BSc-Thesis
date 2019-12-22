using System;

namespace Core.Extensions
{
    public static class IntExtensions
    {
        public static string SecondsToStopwatchTimeString(this int seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            return $"{time.Minutes:D2}:{time.Seconds:D2}";
        }
    }
}