﻿using System;

namespace Api.Entities
{
    public class RouteProperties
    {
        public string Name { get; set; }
        public int PavedPercentage { get; set; }
        public double Distance { get; set; }
        public HeightAboveSeaLevel HeightAboveSeaLevel { get; set; }
    }

    public enum HeightAboveSeaLevel
    {
        Close = 1,
        Increasing,
        Decreasing
    }
}