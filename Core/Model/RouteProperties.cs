using SQLite;

namespace Core.Model
{
    public class RouteProperties
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
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