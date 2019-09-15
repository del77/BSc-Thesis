using System.Collections.Generic;
using Core.Model;

namespace Core.Repositories
{
    public class RoutesRepository
    {
        private static List<Route> _routes = new List<Route>()
        {
            new Route()
            {
                Name = "xd",Distance = 123,
                Checkpoints = new List<Point>
                {
                    new Point(51.949004, 19.205046, 1),
                    new Point(51.949104, 19.205046, 2),
                    new Point(51.949204, 19.205046, 3)
                }
            }
        };

        public RoutesRepository()
        {

        }

        public void CreateRoute(Route route)
        {
            _routes.Add(route);
        }

        public IEnumerable<Route> GetAll()
        {
            return _routes;
        }

        
    }
}