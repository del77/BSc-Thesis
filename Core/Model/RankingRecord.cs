using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Core.Model
{
    public class RankingRecord
    {
        private List<int> _checkpointsTimesList;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int RouteId { get; set; }
        [Ignore]
        public string User { get; set; }
        public string CheckpointsTimes { get; set; }
        public int FinalResult { get; set; }

        [Ignore]
        public bool IsMine { get; set; }

        [Ignore]
        public List<int> CheckpointsTimesList
        {
            get =>
                _checkpointsTimesList ??
                (_checkpointsTimesList = CheckpointsTimes.Split(' ').Select(int.Parse).ToList());
            set => _checkpointsTimesList = value;
        }

        public RankingRecord(string checkpointsTimes, int finalResult)
        {
            CheckpointsTimes = checkpointsTimes;
            FinalResult = finalResult;
        }

        public RankingRecord(List<int> rankingRecordTimes)
        {
            _checkpointsTimesList = rankingRecordTimes;
        }

        public RankingRecord()
        {
        }
    }
}