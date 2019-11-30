using System;
using NetTopologySuite.Geometries;

namespace Api.Entities
{
    public class Point
    {
        public Guid Id { get; set; }

        public Guid RouteId { get; set; }
        public Route Route { get; set; }
        public int Number { get; set; }
        public NetTopologySuite.Geometries.Point Coordinates { get; set; }
    }
}