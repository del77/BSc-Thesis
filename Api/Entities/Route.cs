using System.Collections.Generic;

namespace Api.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public RouteProperties Properties { get; set; }
        public IEnumerable<Point> Checkpoints { get; set; }
        public IEnumerable<RankingRecord> Rankingg { get; set; }
    }
}