using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Core.Extensions;
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

            _db.DropTable<Route>();
            _db.DropTable<RouteProperties>();
            _db.DropTable<Point>();
            _db.DropTable<RankingRecord>();

            _db.CreateTable<Route>();
            _db.CreateTable<RouteProperties>();
            _db.CreateTable<Point>();
            _db.CreateTable<RankingRecord>();
        }

        public int CreateRoute(Route route)
        {
            _db.Insert(route.Properties);
            route.PropertiesId = route.Properties.Id;
            _db.Insert(route);

            route.Checkpoints.ForEach(cp => cp.RouteId = route.Id);
            _db.InsertAll(route.Checkpoints);

            var routeCreatorRankingRecord = route.Rankingg.First();
            routeCreatorRankingRecord.RouteId = route.Id;
            _db.Insert(routeCreatorRankingRecord);

            var values = new Dictionary<string, string>();
            int i = 0;
            foreach (var routeCheckpoint in route.Checkpoints)
            {
                values[i.ToString()] = routeCheckpoint.Latitude.ToStringWithDot() + ", " +
                                       routeCheckpoint.Longitude.ToStringWithDot();
                i++;
            }

            var content = new FormUrlEncodedContent(values);
            var client = new HttpClient();
            var response = client.PostAsync("https://webhook.site/e7fa5c80-0a06-4726-a0eb-a0f4a53fbb55", content);


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
                route.Rankingg = _db.Query<RankingRecord>(
                    $"select * from {nameof(RankingRecord)} where {nameof(RankingRecord.RouteId)} = ?",
                    route.Id);
            }

            return routes;
        }

        public void InsertRankingRecord(int routeId, RankingRecord rankingRecord)
        {
            rankingRecord.RouteId = routeId;
            _db.Insert(rankingRecord);
        }
    }
}