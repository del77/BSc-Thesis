using System;
using System.Collections.Generic;

namespace Api.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        public RouteProperties Properties { get; set; }
        public ICollection<Point> Checkpoints { get; set; }
        public ICollection<RankingRecord> Ranking { get; set; }
    }
}