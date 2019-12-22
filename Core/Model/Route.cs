using System;
using System.Collections.Generic;

namespace Core.Model
{
    public class Route
    {
        public Guid Id { get; set; }
        public RouteProperties Properties { get; set; }
        public List<Point> Checkpoints { get; set; }

        public List<RankingRecord> Ranking { get; set; }

        public Route(string name)
        {
            Checkpoints = new List<Point>();
            Ranking = new List<RankingRecord>();
            Properties = new RouteProperties
            {
                Name = name
            };
        }
    }
}