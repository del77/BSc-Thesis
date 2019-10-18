namespace Core.Model
{
    public class RouteProperties
    {
        public int PavedPercentage { get; set; }
        public double Distance { get; set; }
        public HeightAboveSeaLevel HeightAboveSeaLevel { get; set; }

        public RouteProperties()
        {
            PavedPercentage = 50;
        }
    }

    public enum HeightAboveSeaLevel
    {
        Close,
        Increasing,
        Decreasing
    }
}