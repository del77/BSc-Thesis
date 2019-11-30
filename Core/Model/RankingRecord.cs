using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Core.Model
{
    public class RankingRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int RouteId { get; set; }
        [Ignore]
        public string User { get; set; }

        [Ignore]
        public bool IsMine { get; set; }

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
    }
}