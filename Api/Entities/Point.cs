using System;

namespace Api.Entities
{
    public class Point
    {
        public Guid Id { get; set; }

        public Guid RouteId { get; set; }
        public Route Route { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
    }
}