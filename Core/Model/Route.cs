using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Core.Model
{
    public class Route
    {
        public string Name { get; set; }
        public RouteProperties Properties { get; set; }
        public List<Point> Checkpoints { get; set; }
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
            return new Route
            {
                Name = "Nowa trasa",
                Checkpoints = new List<Point>(),
                Ranking = new List<KeyValuePair<string, List<Point>>>()
            };
        }
    }
}