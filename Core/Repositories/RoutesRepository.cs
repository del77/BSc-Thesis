using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Model;
using SQLite;
using SQLiteNetExtensions;
using SQLiteNetExtensions.Extensions;

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
            _db.DropTable<Route>();
            _db.CreateTable<Route>();
            _db.CreateTable<RouteProperties>();
        }

        public int CreateRoute(Route route)
        {
            route.Ranking = null;
            _db.InsertWithChildren(route);
            return route.Id;
            //return _db.Insert(route);
        }

        public IEnumerable<Route> GetAll()
        {
            return _db.Table<Route>().ToList();
        }

        
    }
}