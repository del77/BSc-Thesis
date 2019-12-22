using System;
using Core.Extensions;
using SQLite;

namespace Core.Model
{
    public class Point
    {
        private const double EarthRadiusInKilometers = 6371;


        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        [Indexed] public int RouteId { get; set; }
        [Indexed] public double Latitude { get; set; }
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
            var latitudeDelta = deg2rad(point1.Latitude - point2.Latitude);
            var longitudeDelta = deg2rad(point1.Longitude - point2.Longitude);

            var firstPointLatitudeCos = Math.Cos(deg2rad(point1.Latitude));
            var secondPointLatitudeCos = Math.Cos(deg2rad(point2.Latitude));

            var a = Math.Pow(Math.Sin(latitudeDelta / 2), 2) + firstPointLatitudeCos *
                    secondPointLatitudeCos * Math.Pow(Math.Sin(longitudeDelta / 2), 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = EarthRadiusInKilometers * c;

            return d;

        }

        public static double Distance(Point point1, Point point2, char unit)
        {
            //https://www.geodatasource.com/developers/c-sharp
            double lat1 = point1.Latitude;
            double lon1 = point1.Longitude;
            double lat2 = point2.Latitude;
            double lon2 = point2.Longitude;


            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) +
                              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }

                return (dist);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private static double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        public static Point ComputeOffset(Point point, double distance, double heading)
        {
            distance /= EarthRadiusInKilometers;
            heading = deg2rad(heading);

            double fromLat = deg2rad(point.Latitude);
            var fromLng = deg2rad(point.Longitude);

            double cosDistance = Math.Cos(distance);
            double sinDistance = Math.Sin(distance);
            double sinFromLat = Math.Sin(fromLat);
            double cosFromLat = Math.Cos(fromLat);
            double sinLat = cosDistance * sinFromLat + sinDistance * cosFromLat * Math.Cos(heading);
            double dLng = Math.Atan2(sinDistance * cosFromLat * Math.Sin(heading), cosDistance - sinFromLat * sinLat);

            return new Point(rad2deg(Math.Asin(sinLat)), rad2deg(fromLng + dLng), 0);
        }

        public static Point GetPointWithGivenKilometersDistanceAndBearingFromStartingPoint(Point startingPoint, double distance,
            double bearing)
        {
            var angularDistance = distance / EarthRadiusInKilometers;


            var startingPointLatitudeSin = Math.Sin(deg2rad(startingPoint.Latitude));
            var startingPointLatitudeCos = Math.Cos(deg2rad(startingPoint.Latitude));
            var angularDistanceSin = Math.Sin(angularDistance);
            var angularDistanceCos = Math.Cos(angularDistance);
            var bearingSin = Math.Sin(deg2rad(bearing));
            var bearingCos = Math.Cos(deg2rad(bearing));


            var newLatitude = rad2deg(Math.Asin(startingPointLatitudeSin * angularDistanceCos +
                                        startingPointLatitudeCos * angularDistanceSin * bearingCos));
            var newLongitude = startingPoint.Longitude + rad2deg(Math.Atan2(
                                   bearingSin * angularDistanceSin * startingPointLatitudeCos,
                                   angularDistanceCos - startingPointLatitudeSin * Math.Sin(newLatitude)));

            return new Point((newLatitude), (newLongitude), null);
        }
    }
}