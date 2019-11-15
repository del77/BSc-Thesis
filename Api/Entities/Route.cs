using System;
using System.Collections.Generic;

namespace Api.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public RouteProperties Properties { get; set; }
        public IEnumerable<Point> Checkpoints { get; set; }
        public IEnumerable<RankingRecord> Ranking { get; set; }
    }
}