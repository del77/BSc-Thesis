using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Model
{
    public class RankingRecord
    {
        public Guid RouteId { get; set; }
        public string User { get; set; }

        public bool IsMine { get; set; }
        public bool CurrentTry { get; set; }

        public List<int> CheckpointsTimes { get; set; }

        public int FinalResult => CheckpointsTimes.Last();

        public RankingRecord(List<int> rankingRecordTimes)
        {
            CheckpointsTimes = rankingRecordTimes;
        }

        public RankingRecord()
        {
            CheckpointsTimes = new List<int>();
        }

        public RankingRecord(bool currentTry, Guid routeId)
        {
            CurrentTry = currentTry;
            RouteId = routeId;
            CheckpointsTimes = new List<int>();
        }
    }
}