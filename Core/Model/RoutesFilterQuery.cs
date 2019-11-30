namespace Core.Model
{
    public class RoutesFilterQuery
    {
        public int RouteLengthFrom { get; set;}
        public int RouteLengthTo { get; set; }

        public int SurfacePavedPercentageFrom { get; set; }
        public int SurfacePavedPercentageTo { get; set; }

        public HeightAboveSeaLevel SurfaceLevel { get; set; }
        public int SearchRadiusInMeters { get; set; }

        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
    }
}
