using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Core.Model
{
    [Table("Routes")]
    public class Route
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [OneToOne]
        public RouteProperties Properties { get; set; }
        [OneToMany]
        public List<Point> Checkpoints { get; set; }
        [OneToMany]
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