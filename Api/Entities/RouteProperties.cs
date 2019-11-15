using System;

namespace Api.Entities
{
    public class RouteProperties
    {
        public Guid RouteId { get; set; }
        public string Name { get; set; }
        public int PavedPercentage { get; set; }
        public double Distance { get; set; }
        public HeightAboveSeaLevel HeightAboveSeaLevel { get; set; }
    }

    public enum HeightAboveSeaLevel
    {
        Close,
        Increasing,
        Decreasing
    }
}