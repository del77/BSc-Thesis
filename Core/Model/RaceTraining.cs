using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Core.Services;

namespace Core.Model
{
    public class RaceTraining : TrainingBase
    {
        private readonly Action _checkpointReached;
        private readonly Action _stopTrainingUi;
        public int NextCheckpointIndex = 0;
        private List<Point> checkpoints;
        private readonly RoutesService _routesService;

        public RaceTraining(Route route, Action uiUpdate, Action checkpointReached, Func<Task<Tuple<double, double, double?>>> currentLocationDelegate, Action stopTrainingUi) : base(route, uiUpdate, currentLocationDelegate)
        {
            _routesService = new RoutesService();
            _checkpointReached = checkpointReached;
            _stopTrainingUi = stopTrainingUi;
        }

        public override async void Start()
        {
            NextCheckpointIndex = 0;

            CurrentTry = new RankingRecord(true);
            Route.Ranking.Add(CurrentTry);

            checkpoints = new List<Point>();
            IsStarted = true;

            Timer = new Timer(1000);
            Timer.Elapsed += _timer_Elapsed;

            AddPoint();
            Timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Seconds++;

            AddPoint();
            UiUpdate();
        }

        public override void Stop()
        {
            Timer.Stop();
            IsStarted = false;

            Seconds = 0;

        }

        private bool xd = true;
        protected override async void AddPoint()
        {
            //if (!xd)
           //    return;
            //xd = false;

            var location = await GetLocation();
            var currentLocation = (new Point(location.Item1, location.Item2, Seconds));
            var distance = Point.Distance(currentLocation, Route.Checkpoints[NextCheckpointIndex], 'K');

            Points.Add(currentLocation);

            if (distance < 0.005)
            {
                if (NextCheckpointIndex++ > 0)
                    SaveCheckpointTime();

                if (NextCheckpointIndex == Route.Checkpoints.Count)
                {
                    Stop();
                    //CurrentTry.CheckpointsTimes[CurrentTry.CheckpointsTimes.Count-1] = 13;
                    _routesService.ProcessCurrentTry(Route, CurrentTry);
                    await _routesService.UpdateRankingAsync(Route.Id, CurrentTry);
                    _stopTrainingUi.Invoke();

                }
                else
                {
                    _checkpointReached.Invoke();
                }
            }

            // xd = true;
        }
    }
}