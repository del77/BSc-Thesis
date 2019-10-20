using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Model;
using SQLite;

namespace Core.Repositories
{
    public class RoutesRepository
    {
        private readonly SQLite.SQLiteConnection _db;

        private static List<Route> _routes = new List<Route>()
        {
            new Route()
            {
                Properties =
                {
                    Name = "xd",
                    Distance = 2.69d,
                    HeightAboveSeaLevel = HeightAboveSeaLevel.Increasing,
                    PavedPercentage = 75,
                },
                Checkpoints = new List<Point>
                {
                    new Point(51.752521, 19.438490, 123, 1),
                    new Point(51.752295, 19.439370, 123, 2),
                    new Point(51.752209, 19.441955, 123, 3)
                }
            }
        };

        public RoutesRepository()
        {
            var databasePath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "database.db");
            _db = new SQLiteConnection(databasePath);

            //_db.DropTable<Route>();
            //_db.DropTable<RouteProperties>();
            //_db.DropTable<Point>();

            _db.CreateTable<Route>();
            _db.CreateTable<RouteProperties>();
            _db.CreateTable<Point>();
        }

        public int CreateRoute(Route route)
        {
            route.PropertiesId = _db.Insert(route.Properties);
            var routeId = _db.Insert(route);

            route.Checkpoints.ForEach(cp => cp.RouteId = routeId);
            _db.InsertAll(route.Checkpoints);

            return route.Id;
        }

        public IEnumerable<Route> GetAll()
        {
            var xd = _db.Table<RouteProperties>();
            var routes = _db.Table<Route>().ToList();
            foreach (var route in routes)
            {
                route.Properties =
                    _db.Query<RouteProperties>(
                        $"select * from {nameof(RouteProperties)} where {nameof(RouteProperties.Id)} = ?",
                        route.PropertiesId).SingleOrDefault();
                route.Checkpoints = _db.Query<Point>($"select * from {nameof(Point)} where {nameof(Point.RouteId)} = ?",
                    route.Id);
            }

            return routes;
        }

        
    }
}