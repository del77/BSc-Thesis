﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Core.Repositories;

namespace Core.Model
{
    public class Training
    {
        private Timer _timer;
        private Action _uiUpdate;
        private Func<Task<Tuple<double, double>>> _getLocation;

        public bool IsStarted { get; set; }
        public int Seconds { get; set; }
        public List<Point> Points { get; set; }
        private readonly Route _route;

        private readonly RoutesRepository _routesRepository;

        public Training(Route route)
        {
            Points = new List<Point>();
            _route = route;
            _routesRepository = new RoutesRepository();
        }

        public void Start(Action uiUpdate, Func<Task<Tuple<double, double>>> getCurrentLocation)
        {
            IsStarted = true;
            _uiUpdate = uiUpdate;
            _getLocation = getCurrentLocation;

            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

        }

        private List<Point> RetrieveCheckpoints()
        {
            var lastPointToCheckIndex = Points.Count - 2;
            var distanceBetweenCheckpoints = 10; //meters
            var retrievedCheckpoints = new List<Point>();

            retrievedCheckpoints.Add(Points.First()); //starting point

            for (int i = 0; i < lastPointToCheckIndex;)
            {
                for (int j = i + 1; j <= lastPointToCheckIndex; j++)
                {
                    if (GetDistanceBetweenPoints(Points[i], Points[j]) > distanceBetweenCheckpoints)
                    {
                        i = j;
                        retrievedCheckpoints.Add(retrievedCheckpoints[j]);
                        break;
                    }

                    if (j == lastPointToCheckIndex)
                        i = lastPointToCheckIndex;
                }
            }

            retrievedCheckpoints.Add(Points.Last()); // ending point
            return retrievedCheckpoints;
        }

        private async void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Seconds++;
            var location = await _getLocation();
            Points.Add(new Point(location.Item1, location.Item2, Seconds));

            _uiUpdate();
        }

        public void Stop()
        {
            IsStarted = false;
            _timer.Stop();
            Seconds = 0;

            _route.Distance = GetTrainingDistance();
            _route.Checkpoints = RetrieveCheckpoints();
            _route.Ranking.Add(new KeyValuePair<string, List<Point>>("Anon", Points));

            _routesRepository.CreateRoute(_route);
        }

        private double GetTrainingDistance()
        {
            var totalDistance = 0d;
            for (int i = 0; i < Points.Count - 1; i++)
            {
                totalDistance += GetDistanceBetweenPoints(Points[i], Points[i + 1]);
            }

            return totalDistance;
        }

        private double GetDistanceBetweenPoints(Point point1, Point point2)
        {
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
            return distance / 1000;
        }
    }
}