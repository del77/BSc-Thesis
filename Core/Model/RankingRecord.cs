using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Core.Model
{
    public class RankingRecord
    {
        [PrimaryKey, AutoIncrement]
        public Guid Id { get; set; }
        [Indexed]
        public Guid RouteId { get; set; }
        [Ignore]
        public string User { get; set; }

        [Ignore]
        public bool IsMine { get; set; }
        [Ignore]
        public bool CurrentTry { get; set; }

        [Ignore]
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