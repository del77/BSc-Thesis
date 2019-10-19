using System;
using Core.Extensions;

namespace Core.Model
{
    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public int Time { get; set; }

        public Point(double latitude, double longitude, double? altitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
        }

        public Point(double latitude, double longitude, double? altitude, int time) : this(latitude, longitude, altitude)
        {
            Time = time;
        }

        protected Point() { }

        public override string ToString()
        {
            return $"{Latitude.ToStringWithDot()}, {Longitude.ToStringWithDot()}";
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
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
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
            var earthRadius = 6371009;
            distance /= earthRadius;
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

        /*
         * public static LatLng computeOffset(LatLng from, double distance, double heading) {
    distance /= 6371009.0D;  //earth_radius = 6371009 # in meters
    heading = Math.toRadians(heading);
    double fromLat = Math.toRadians(from.latitude);
    double fromLng = Math.toRadians(from.longitude);
    double cosDistance = Math.cos(distance);
    double sinDistance = Math.sin(distance);
    double sinFromLat = Math.sin(fromLat);
    double cosFromLat = Math.cos(fromLat);
    double sinLat = cosDistance * sinFromLat + sinDistance * cosFromLat * Math.cos(heading);
    double dLng = Math.atan2(sinDistance * cosFromLat * Math.sin(heading), cosDistance - sinFromLat * sinLat);
    return new LatLng(Math.toDegrees(Math.asin(sinLat)), Math.toDegrees(fromLng + dLng));
         */
    }


}