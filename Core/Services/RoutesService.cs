using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core.Model;
using Core.Repositories;

namespace Core.Services
{
    public class RoutesService
    {
        private readonly RoutesRepository _routesRepository;
        private readonly RoutesWebRepository _routesWebRepository;
        private readonly UserRepository _userRepository;
        private readonly UserData _userData;

        public RoutesService()
        {
            _routesWebRepository = new RoutesWebRepository();
            _routesRepository = new RoutesRepository();
            _userRepository = new UserRepository();
            _userData = _userRepository.GetUserData();
        }

        public async Task CreateRoute(Route route)
        {
            //_routesRepository.CreateRoute(route);
            await _routesWebRepository.CreateRoute(route);

            await Task.CompletedTask;
        }

        public async Task UpdateRanking(Route route, RankingRecord currentTry)
        {
            var lastTry = route.Ranking.SingleOrDefault(rr => rr.IsMine);
            if (lastTry != null)
            {
                if (currentTry.FinalResult < lastTry.FinalResult)
                {
                    route.Ranking.Remove(lastTry);
                    currentTry.IsMine = true;
                }
                else
                    route.Ranking.Remove(currentTry);
            }

            await _routesWebRepository.CreateRankingRecordAsync(currentTry, route.Id);


            //int firstWorseTryIndex = route.Rankingg.FindIndex(r => r.Points.Last().Time >= rankingRecord.Points.Last().Time);
            //int firstWorseTryIndex = route.Rankingg.FindIndex(r => r.FinalResult >= ro);
            //if (firstWorseTryIndex == -1)
            //    firstWorseTryIndex = route.Rankingg.Count;

            ////route.Ranking.Insert(firstWorseTryIndex, new KeyValuePair<string, List<Point>>("Anon2", rankingRecord.Points.ToList()));
            //var record = new RankingRecord("Anon1234", routeTimes);
            //.InsertRankingRecord(route.Id, route.Ranking.Single(r => r.Id == 0));

            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Route>> GetRoutes(RoutesFilterQuery query)
        {
            var routes = (await _routesWebRepository.GetRoutes(query)).ToList();
            foreach (var route in routes)
            {
                var mineRankingRecord = route.Ranking.FirstOrDefault(r => r.User == _userData.Username);
                if (mineRankingRecord != null)
                    mineRankingRecord.IsMine = true;
            }

            return routes;
        }

    }
}