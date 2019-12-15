using System;
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

        public async Task<bool> CreateRoute(Route route)
        {
            //_routesRepository.CreateRoute(route);
            return await _routesWebRepository.CreateRoute(route);
        }

        public void ProcessCurrentTry(Route route, RankingRecord currentTry)
        {
            var lastTry = route.Ranking.SingleOrDefault(rr => rr.IsMine);
            if (lastTry != null)
            {
                if (currentTry.FinalResult < lastTry.FinalResult)
                {
                    currentTry.IsMine = true;
                    currentTry.CurrentTry = false;
                    currentTry.User = lastTry.User;

                    route.Ranking.Remove(lastTry);
                }
                else
                    route.Ranking.Remove(currentTry);
            }

            //await _routesWebRepository.CreateRankingRecordAsync(currentTry, route.Id);


            //int firstWorseTryIndex = route.Rankingg.FindIndex(r => r.Points.Last().Time >= rankingRecord.Points.Last().Time);
            //int firstWorseTryIndex = route.Rankingg.FindIndex(r => r.FinalResult >= ro);
            //if (firstWorseTryIndex == -1)
            //    firstWorseTryIndex = route.Rankingg.Count;

            ////route.Ranking.Insert(firstWorseTryIndex, new KeyValuePair<string, List<Point>>("Anon2", rankingRecord.Points.ToList()));
            //var record = new RankingRecord("Anon1234", routeTimes);
            //.InsertRankingRecord(route.Id, route.Ranking.Single(r => r.Id == 0));

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

        public async Task<bool> UpdateRankingAsync(Guid routeId, RankingRecord currentTry)
        {
            return await _routesWebRepository.CreateRankingRecordAsync(currentTry, routeId);
        }

        public async Task ProcessOverdueData()
        {
            var dataToSend = _userRepository.GetDataToSend();

            foreach (var data in dataToSend)
            {
                var result = await _routesWebRepository.SendJsonData(data.Json, data.Uri);

                if (result)
                    _userRepository.DeleteDataToSend(data.Id);
            }
        }
    }
}