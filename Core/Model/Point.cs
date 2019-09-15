namespace Core.Model
{
    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Time { get; set; }

        public Point(double latitude, double longitude, int time)
        {
            Latitude = latitude;
            Longitude = longitude;
            Time = time;
        }
    }

    
}