using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Core.Model
{
    public class InitialTraining : TrainingBase
    {
        readonly double _kilometersDistanceBetweenCheckpoints = 0.01;
        private int _checkpointNumber;

        public InitialTraining(Route route, Action uiUpdate, Func<Task<Tuple<double, double, double?>>> currentLocationDelegate) : base(route, uiUpdate, currentLocationDelegate)
        {
        }

        public override void Start()
        {
            IsStarted = true;

            _checkpointNumber = 0;
            CurrentTry = new RankingRecord();
            Route.Ranking.Add(CurrentTry);

            AddPoint();

            Timer = new Timer(1000);
            Timer.Elapsed += _timer_Elapsed;

            Timer.Start();
        }

        public override void Stop()
        {
            Timer.Stop();
            IsStarted = false;

            var location = GetLocation().GetAwaiter().GetResult();
            Route.Checkpoints.Add(new Point(location.Item1, location.Item2, location.Item3, _checkpointNumber));
            SaveCheckpointTime();

            Seconds = 0;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Seconds++;
            AddPoint();

            UiUpdate();
        }

        protected override async void AddPoint()
        {
            var location = await GetLocation();
            var currentPosition = new Point(location.Item1, location.Item2, location.Item3, _checkpointNumber);
            var distanceFromLastPoint = Route.Checkpoints.Any()
                ? Point.HaversineKilometersDistance(Route.Checkpoints.Last(), currentPosition)
                : _kilometersDistanceBetweenCheckpoints;
            if (distanceFromLastPoint >= _kilometersDistanceBetweenCheckpoints)
            {
                _checkpointNumber++;
                Route.Checkpoints.Add(currentPosition);
                SaveCheckpointTime();
            }
        }
    }
}