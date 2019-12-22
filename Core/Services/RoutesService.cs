using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.Repositories;

namespace Core.Services
{
    public class RoutesService
    {
        private readonly RoutesWebRepository _routesWebRepository;
        private readonly UserRepository _userRepository;
        private readonly UserData _userData;

        public RoutesService()
        {
            _routesWebRepository = new RoutesWebRepository();
            _userRepository = new UserRepository();
            _userData = _userRepository.GetUserData();
        }

        public async Task<bool> CreateRoute(Route route)
        {
            return await _routesWebRepository.CreateRoute(route);
        }

        public void ProcessCurrentTry(Route route, RankingRecord currentTry)
        {
            var lastTry = route.Ranking.SingleOrDefault(rr => rr.IsMine);
            if (lastTry == null) 
                return;
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