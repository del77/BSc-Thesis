using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Core.Model;

namespace Core.Training
{
    public class InitialTraining : TrainingBase
    {
        readonly double _kilometersDistanceBetweenCheckpoints = 0.015;


        public InitialTraining(Route route, Action uiUpdate, Func<Task<Tuple<double, double, double?>>> currentLocationDelegate) : base(route, uiUpdate, currentLocationDelegate)
        {
        }

        public override void Stop()
        {
            base.Stop();
            var location = GetLocation().GetAwaiter().GetResult();
            Route.Checkpoints.Add(new Point(location.Item1, location.Item2, location.Item3, NextCheckpointIndex));
            RoutesService.CalculateRouteDistance(Route);
            SaveCheckpointTime();
        }

        protected override async void ProcessUserLocation()
        {
            var location = await GetLocation();
            var currentPosition = new Point(location.Item1, location.Item2, location.Item3, NextCheckpointIndex);
            var distanceFromLastPoint = Route.Checkpoints.Any()
                ? Point.HaversineKilometersDistance(Route.Checkpoints.Last(), currentPosition)
                : _kilometersDistanceBetweenCheckpoints;
            if (distanceFromLastPoint >= _kilometersDistanceBetweenCheckpoints)
            {
                NextCheckpointIndex++;
                Route.Checkpoints.Add(currentPosition);
                SaveCheckpointTime();
            }
        }
    }
}