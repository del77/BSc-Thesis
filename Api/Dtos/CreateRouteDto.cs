using System.Collections.Generic;
using Api.Entities;

namespace Api.Dtos
{
    public class CreateRouteDto
    {
        public RoutePropertiesDto Properties { get; set; }
        public IEnumerable<PointDto> Checkpoints { get; set; }
        public IEnumerable<RankingRecordDto> Ranking { get; set; }
    }

    public class RoutePropertiesDto
    {
        public string Name { get; set; }
        public int PavedPercentage { get; set; }
        public double Distance { get; set; }
        public HeightAboveSeaLevel HeightAboveSeaLevel { get; set; }
    }

    public class PointDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
    }

    public class RankingRecordDto
    {
        public IEnumerable<int> CheckpointsTimes { get; set; }
        public int FinalResult { get; set; }
    }
}