﻿using Api.Entities;

namespace Api.Dtos
{
    public class RoutesQuery
    {
        public int RouteLengthFrom { get; set; }
        public int RouteLengthTo { get; set; }

        public int SurfacePavedPercentageFrom { get; set; }
        public int SurfacePavedPercentageTo { get; set; }

        public int SurfaceLevel { get; set; }
        public int SearchRadiusInMeters { get; set; }

        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
    }
}