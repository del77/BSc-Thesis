using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Model
{
    public class Route
    {
        public Guid Id { get; set; }
        public RouteProperties Properties { get; set; }
        public List<Point> Checkpoints { get; set; }

        public List<RankingRecord> Ranking { get; set; }

        public Route(string name)
        {
            Checkpoints = new List<Point>();
            Ranking = new List<RankingRecord>();
            Properties = new RouteProperties
            {
                Name = name
            };
        }

        public void CalculateDistance()
        {
            var totalDistance = 0d;

            for (int i = 0; i < Checkpoints.Count - 1; i++)
            {
                totalDistance += Point.HaversineKilometersDistance(Checkpoints[i], Checkpoints[i + 1]);
            }

            Properties.Distance = Math.Round(totalDistance, 2);
        }

        public TerrainLevel ResolveTerrainLevel()
        {
            const double terrainLevelDifferenceThreshold = 1.1;
            var startAltitude = Checkpoints.First().Altitude;
            var finishAltitude = Checkpoints.Last().Altitude;

            Properties.TerrainLevel = TerrainLevel.Close;
            if (startAltitude != null && finishAltitude != null)
            {
                if (startAltitude > finishAltitude * terrainLevelDifferenceThreshold)
                {
                    Properties.TerrainLevel = TerrainLevel.Decreasing;
                }
                else if (finishAltitude > startAltitude * terrainLevelDifferenceThreshold)
                {
                    Properties.TerrainLevel = TerrainLevel.Increasing;
                }
            }

            return Properties.TerrainLevel;
        }
    }
}