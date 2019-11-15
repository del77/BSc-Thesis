using System;

namespace Api.Entities
{
    public class RankingRecord
    {
        public Guid RouteId { get; set; }
        public Route Route { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public string CheckpointsTimes { get; set; }
        public int FinalResult { get; set; }

    }
}