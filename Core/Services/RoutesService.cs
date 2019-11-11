using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Repositories;

namespace Core.Services
{
    public class RoutesService
    {
        private readonly RoutesRepository _routesRepository;
        private readonly UserRepository _userRepository;
        private readonly UserData _userData;

        public RoutesService()
        {
            _routesRepository = new RoutesRepository();
            _userRepository = new UserRepository();
            _userData = _userRepository.GetUserData();
        }

        public async Task CreateRoute(Route route)
        {
            route.Rankingg.First().User = _userData;
            _routesRepository.CreateRoute(route);

            await Task.CompletedTask;
        }

        public async Task UpdateRanking(Route route)
        {
            //int firstWorseTryIndex = route.Rankingg.FindIndex(r => r.Points.Last().Time >= rankingRecord.Points.Last().Time);
            //int firstWorseTryIndex = route.Rankingg.FindIndex(r => r.FinalResult >= ro);
            //if (firstWorseTryIndex == -1)
            //    firstWorseTryIndex = route.Rankingg.Count;

            ////route.Ranking.Insert(firstWorseTryIndex, new KeyValuePair<string, List<Point>>("Anon2", rankingRecord.Points.ToList()));
            //var record = new RankingRecord("Anon1234", routeTimes);
            _routesRepository.InsertRankingRecord(route.Id, route.Rankingg.Single(r => r.Id == 0));

            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Route>> GetAllRoutes()
        {
            var routes = _routesRepository.GetAll().ToList();
            foreach (var route in routes)
            {
                var mineRankingRecord = route.Rankingg.FirstOrDefault(r => r.User.PhoneNumber == _userData.PhoneNumber);
                if (mineRankingRecord != null)
                    mineRankingRecord.IsMine = true;
            }

            return routes;
        }

    }
}