using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Core.Model;
using Core.Services;

namespace Core.Training
{
    public abstract class TrainingBase
    {
        public bool IsStarted { get; set; }
        public int Seconds { get; set; }
        public List<Point> Points { get; set; }
        
        protected readonly RoutesService RoutesService;
        protected int NextCheckpointIndex = 0;
        protected readonly Route Route;
        protected Timer Timer;
        protected Action UiUpdate;
        protected Func<Task<Tuple<double, double, double?>>> GetLocation;
        protected RankingRecord CurrentTry { get; set; }


        protected TrainingBase(Route route, Action uiUpdate, Func<Task<Tuple<double, double, double?>>> currentLocationDelegate)
        {
            Route = route;
            UiUpdate = uiUpdate;
            GetLocation = currentLocationDelegate;
            Points = new List<Point>();
            CurrentTry = new RankingRecord();
            RoutesService = new RoutesService();
        }

        protected void SaveCheckpointTime()
        {
            CurrentTry.CheckpointsTimes.Add(Seconds);
        }

        public void Start()
        {
            IsStarted = true;
            CurrentTry = new RankingRecord(true, Route.Id);
            Route.Ranking.Add(CurrentTry);


            Timer = new Timer(1000);
            Timer.Elapsed += _timer_Elapsed;

            ProcessUserLocation();
            Timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Seconds++;

            ProcessUserLocation();
            UiUpdate();
        }

        public virtual void Stop()
        {
            Timer.Stop();
            IsStarted = false;
            Seconds = 0;
        }

        protected abstract void ProcessUserLocation();
    }
}