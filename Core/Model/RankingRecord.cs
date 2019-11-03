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
        public string User { get; set; }
        public string CheckpointsTimes { get; set; }
        public int FinalResult { get; set; }
        public RankingRecord(string user, string checkpointsTimes)
        {
            User = user;
            CheckpointsTimes = checkpointsTimes;
        }

        public RankingRecord(string checkpointsTimes, int finalResult)
        {
            CheckpointsTimes = checkpointsTimes;
            FinalResult = finalResult;
        }

        public RankingRecord()
        {
            
        }
    }
}