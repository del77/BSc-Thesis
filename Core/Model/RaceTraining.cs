using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Core.Services;

namespace Core.Model
{
    public class RaceTraining : TrainingBase
    {
        private readonly Action _checkpointReached;
        private readonly Action _stopTrainingUi;
        private readonly Action<int> _playCurrentPosition;
        private readonly Action<int> _playPositionsLost;
        private readonly Action<int> _playPositionsEarned;
        private readonly Action _showProgressBar;
        private readonly Action _hideProgressBar;
        private readonly Action<bool> _showIsTryUpdateSuccessful;
        public int NextCheckpointIndex = 0;
        private List<Point> checkpoints;
        private readonly RoutesService _routesService;

        private int _rankingPositionOnPreviousCheckpoint;

        public RaceTraining(Route route, Action uiUpdate, Action checkpointReached,
            Func<Task<Tuple<double, double, double?>>> currentLocationDelegate,
            Action stopTrainingUi, Action<int> playCurrentPosition,
            Action<int> playPositionsLost, Action<int> playPositionsEarned,
            Action showProgressBar, Action hideProgressBar, Action<bool> showIsTryUpdateSuccessful) 
            : base(route, uiUpdate, currentLocationDelegate)
        {
            _routesService = new RoutesService();
            _checkpointReached = checkpointReached;
            _stopTrainingUi = stopTrainingUi;
            _playCurrentPosition = playCurrentPosition;
            _playPositionsLost = playPositionsLost;
            _playPositionsEarned = playPositionsEarned;
            _showProgressBar = showProgressBar;
            _hideProgressBar = hideProgressBar;
            _showIsTryUpdateSuccessful = showIsTryUpdateSuccessful;
        }

        public override async void Start()
        {
            NextCheckpointIndex = 0;

            CurrentTry = new RankingRecord(true, Route.Id);
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
            if (NextCheckpointIndex != Route.Checkpoints.Count)
                Route.Ranking.Remove(CurrentTry);
            Seconds = 0;

        }


        protected override async void AddPoint()
        {
            var location = await GetLocation();
            var currentLocation = (new Point(location.Item1, location.Item2, Seconds));
            var distance = Point.Distance(currentLocation, Route.Checkpoints[NextCheckpointIndex], 'K');

            Points.Add(currentLocation);

            if (distance < 0.005)
            {
                NextCheckpointIndex++;

                SaveCheckpointTime();
                if (NextCheckpointIndex == Route.Checkpoints.Count)
                {
                    Stop();
                    //CurrentTry.CheckpointsTimes[CurrentTry.CheckpointsTimes.Count-1] = 13;
                    _routesService.ProcessCurrentTry(Route, CurrentTry);

                    _stopTrainingUi.Invoke();

                    _showProgressBar.Invoke();
                    var isSuccessful = await _routesService.UpdateRankingAsync(Route.Id, CurrentTry);
                    _hideProgressBar.Invoke();
                    _showIsTryUpdateSuccessful.Invoke(isSuccessful);
                }
                else
                {
                    UpdateRankingToShowPositionsForCheckpoint(NextCheckpointIndex - 1);

                    var currentPosition = GetCurrentPositionInRanking();
                    if (currentPosition == _rankingPositionOnPreviousCheckpoint || _rankingPositionOnPreviousCheckpoint == 0)
                    {
                        _playCurrentPosition.Invoke(currentPosition);
                    }
                    else
                    {
                        if (currentPosition < _rankingPositionOnPreviousCheckpoint)
                            _playPositionsEarned(_rankingPositionOnPreviousCheckpoint - currentPosition);
                        else
                            _playPositionsLost(currentPosition - _rankingPositionOnPreviousCheckpoint);
                    }

                    _rankingPositionOnPreviousCheckpoint = currentPosition;

                    _checkpointReached.Invoke();
                }
            }

            // xd = true;
        }

        private int GetCurrentPositionInRanking()
        {
            return Route.Ranking.FindIndex(rr => rr.CurrentTry) + 1;
        }

        private void UpdateRankingToShowPositionsForCheckpoint(int checkpointIndex)
        {
            Route.Ranking = Route.Ranking.OrderBy(x => x.CheckpointsTimes[checkpointIndex]).ToList();
        }
    }
}