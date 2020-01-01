using System;

namespace Api.Entities
{
    public class RouteProperties
    {
        public string Name { get; set; }
        public int PavedPercentage { get; set; }
        public double Distance { get; set; }
        public TerrainLevel TerrainLevel { get; set; }
    }

    public enum TerrainLevel
    {
        Close = 1,
        Increasing,
        Decreasing
    }
}