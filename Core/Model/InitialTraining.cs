﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Core.Repositories;

namespace Core.Model
{
    public class InitialTraining : TrainingBase
    {
        double distanceBetweenCheckpoints = 0.01; //kilometers
        private int _checkpointNumber;

        public InitialTraining(Route route, Action uiUpdate, Func<Task<Tuple<double, double, double?>>> currentLocationDelegate) : base(route, uiUpdate, currentLocationDelegate)
        {
        }

        public override async void Start()
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

        public override async void Stop()
        {
            Timer.Stop();
            IsStarted = false;

            //Route.Checkpoints = RetrieveCheckpoints();
            var location = GetLocation().GetAwaiter().GetResult();
            Route.Checkpoints.Add(new Point(location.Item1, location.Item2, location.Item3, _checkpointNumber));
            SaveCheckpointTime();

            //Route.Ranking.Add(new KeyValuePair<string, List<Point>>("Anon", Route.Checkpoints));
            //Route.Rankingg.Add(new RankingRecord(_routeTimes.ToString(), Seconds));
            Seconds = 0;
        }

        private List<Point> RetrieveCheckpoints()
        {
            var lastPointToCheckIndex = Points.Count - 2;
            var retrievedCheckpoints = new List<Point>();

            retrievedCheckpoints.Add(Points.First()); //starting point

            for (int i = 0; i < lastPointToCheckIndex;)
            {
                for (int j = i + 1; j <= lastPointToCheckIndex; j++)
                {
                    if (Point.Distance(Points[i], Points[j], 'K') > distanceBetweenCheckpoints)
                    {
                        i = j;

                        retrievedCheckpoints.Add(Points[j]);
                        break;
                    }

                    if (j == lastPointToCheckIndex)
                        i = lastPointToCheckIndex;
                }
            }

            retrievedCheckpoints.Add(Points.Last()); // ending point
            return retrievedCheckpoints;
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
                ? Point.Distance(Route.Checkpoints.Last(), currentPosition, 'K')
                : distanceBetweenCheckpoints;
            if (distanceFromLastPoint >= distanceBetweenCheckpoints)
            {
                _checkpointNumber++;
                Route.Checkpoints.Add(currentPosition);
                SaveCheckpointTime();
            }
        }


        private double GetDistanceBetweenPoints(Point point1, Point point2)
        {
            //http://www.consultsarath.com/contents/articles/KB000012-distance-between-two-points-on-globe--calculation-using-cSharp.aspx
            //Haversine formula
            double lat1 = point1.Latitude;
            double long1 = point1.Longitude;
            double lat2 = point2.Latitude;
            double long2 = point2.Longitude;

            double distance = 0;

            double dLat = (lat2 - lat1) / 180 * Math.PI;
            double dLong = (long2 - long1) / 180 * Math.PI;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                       + Math.Cos(lat2) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(lat1 / 180 * Math.PI), 2);
            //Denominator part of the function
            double dr = Math.Pow(radiusE * Math.Cos(lat1 / 180 * Math.PI), 2)
                        + Math.Pow(radiusP * Math.Sin(lat1 / 180 * Math.PI), 2);
            double radius = Math.Sqrt(nr / dr);

            //Calaculate distance in metres.
            distance = radius * c;
            return distance;
        }

        
    }
}