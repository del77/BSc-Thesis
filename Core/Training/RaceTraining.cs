using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Repositories.Web;
using Core.Services;

namespace Core.Training
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
        private readonly RoutesWebRepository _routesWebRepository;

        private int _rankingPositionOnPreviousCheckpoint;
        readonly double _distanceFromCheckpointToleranceInKilometers = 0.010;

        public RaceTraining(Route route, Action uiUpdate, Action checkpointReached,
            Func<Task<Tuple<double, double, double?>>> currentLocationDelegate,
            Action stopTrainingUi, Action<int> playCurrentPosition,
            Action<int> playPositionsLost, Action<int> playPositionsEarned,
            Action showProgressBar, Action hideProgressBar, Action<bool> showIsTryUpdateSuccessful) 
            : base(route, uiUpdate, currentLocationDelegate)
        {
            _routesWebRepository = new RoutesWebRepository();
            _checkpointReached = checkpointReached;
            _stopTrainingUi = stopTrainingUi;
            _playCurrentPosition = playCurrentPosition;
            _playPositionsLost = playPositionsLost;
            _playPositionsEarned = playPositionsEarned;
            _showProgressBar = showProgressBar;
            _hideProgressBar = hideProgressBar;
            _showIsTryUpdateSuccessful = showIsTryUpdateSuccessful;
        }

        public override void Stop()
        {
            base.Stop();
            if (NextCheckpointIndex != Route.Checkpoints.Count)
                Route.Ranking.Remove(CurrentTry);
        }


        protected override async void ProcessUserLocation()
        {
            var location = await GetLocation();
            var currentLocation = (new Point(location.Item1, location.Item2, Seconds));
            var distance = Point.HaversineKilometersDistance(currentLocation, Route.Checkpoints[NextCheckpointIndex]);

            if (distance < _distanceFromCheckpointToleranceInKilometers)
            {
                NextCheckpointIndex++;

                SaveCheckpointTime();
                if (NextCheckpointIndex == Route.Checkpoints.Count)
                {
                    Stop();
                    RoutesService.ProcessCurrentTry(Route, CurrentTry);

                    _stopTrainingUi.Invoke();

                    _showProgressBar.Invoke();
                    var isSuccessful = await _routesWebRepository.CreateRankingRecordAsync(CurrentTry, Route.Id);
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

        }

        private int GetCurrentPositionInRanking()
        {
            return Route.Ranking.FindIndex(rr => rr.IsCurrentTry) + 1;
        }

        private void UpdateRankingToShowPositionsForCheckpoint(int checkpointIndex)
        {
            Route.Ranking = Route.Ranking.OrderBy(x => x.CheckpointsTimes[checkpointIndex]).ToList();
        }
    }
}