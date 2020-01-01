using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Core.Model;

namespace Core.Training
{
    public abstract class TrainingBase
    {
        public bool IsStarted { get; set; }
        public int Seconds { get; set; }
        public List<Point> Points { get; set; }

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
        }

        protected void SaveCheckpointTime()
        {
            CurrentTry.CheckpointsTimes.Add(Seconds);
        }

        public abstract void Start();
        public abstract void Stop();

        protected abstract void AddPoint();
    }
}