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
        public bool IsCurrentTry { get; set; }

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

        public RankingRecord(bool isCurrentTry, Guid routeId)
        {
            IsCurrentTry = isCurrentTry;
            RouteId = routeId;
            CheckpointsTimes = new List<int>();
        }
    }
}