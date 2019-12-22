using System;
using Core.Extensions;

namespace Core.Model
{
    public class Point
    {
        private const double EarthRadiusInKilometers = 6371;


        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public int Number { get; set; }

        public Point(double latitude, double longitude, double? altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public Point(double latitude, double longitude, double? altitude, int number) : this(latitude, longitude,
            altitude)
        {
            Number = number;
        }

        public Point()
        {
        }

        public override string ToString()
        {
            return $"{Latitude.ToStringWithDot()}, {Longitude.ToStringWithDot()}";
        }

        public static double HaversineKilometersDistance(Point point1, Point point2)
        {
            var latitudeDelta = DegreesToRadians(point1.Latitude - point2.Latitude);
            var longitudeDelta = DegreesToRadians(point1.Longitude - point2.Longitude);

            var firstPointLatitudeCos = Math.Cos(DegreesToRadians(point1.Latitude));
            var secondPointLatitudeCos = Math.Cos(DegreesToRadians(point2.Latitude));

            var a = Math.Pow(Math.Sin(latitudeDelta / 2), 2) + firstPointLatitudeCos *
                    secondPointLatitudeCos * Math.Pow(Math.Sin(longitudeDelta / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = EarthRadiusInKilometers * c;

            return d;

        }

        
        private static double DegreesToRadians(double degrees)
        {
            return (degrees * Math.PI / 180.0);
        }

        private static double RadiansToDegrees(double radians)
        {
            return (radians / Math.PI * 180.0);
        }

        public static Point GetPointWithGivenKilometersDistanceAndBearingFromStartingPoint(Point startingPoint, double distance,
            double bearing)
        {
            var angularDistance = distance / EarthRadiusInKilometers;


            var startingPointLatitudeSin = Math.Sin(DegreesToRadians(startingPoint.Latitude));
            var startingPointLatitudeCos = Math.Cos(DegreesToRadians(startingPoint.Latitude));
            var angularDistanceSin = Math.Sin(angularDistance);
            var angularDistanceCos = Math.Cos(angularDistance);
            var bearingSin = Math.Sin(DegreesToRadians(bearing));
            var bearingCos = Math.Cos(DegreesToRadians(bearing));


            var newLatitude = RadiansToDegrees(Math.Asin(startingPointLatitudeSin * angularDistanceCos +
                                        startingPointLatitudeCos * angularDistanceSin * bearingCos));
            var newLongitude = startingPoint.Longitude + RadiansToDegrees(Math.Atan2(
                                   bearingSin * angularDistanceSin * startingPointLatitudeCos,
                                   angularDistanceCos - startingPointLatitudeSin * Math.Sin(newLatitude)));

            return new Point((newLatitude), (newLongitude), null);
        }
    }
}