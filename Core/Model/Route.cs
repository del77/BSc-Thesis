using System.Collections.Generic;
using SQLite;

namespace Core.Model
{
    public class Route
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int PropertiesId { get; set; }
        [Ignore]
        public RouteProperties Properties { get; set; }
        [Ignore]
        public List<Point> Checkpoints { get; set; }
        [Ignore]
        public List<KeyValuePair<string, List<Point>>> Ranking { get; set; }

        public Route()
        {
            //todo make this ctr protected
            Checkpoints = new List<Point>();
            Ranking = new List<KeyValuePair<string, List<Point>>>();
            Properties = new RouteProperties();
        }

        public static Route GetNewRoute()
        {
            var properties = new RouteProperties
            {
                Name = "Nowa trasa"
            };
            return new Route
            {
                Properties = properties,
                Checkpoints = new List<Point>(),
                Ranking = new List<KeyValuePair<string, List<Point>>>()
            };
        }
    }
}