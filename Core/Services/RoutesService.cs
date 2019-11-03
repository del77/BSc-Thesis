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
        public RoutesService()
        {
            _routesRepository = new RoutesRepository();
        }

        public async Task CreateRoute(Route route)
        {
            route.Rankingg.First().User = "Anon1234";
            _routesRepository.CreateRoute(route);

            await Task.CompletedTask;
        }

        public async Task UpdateRanking(Route route, string routeTimes, int finalResult)
        {
            //int firstWorseTryIndex = route.Rankingg.FindIndex(r => r.Points.Last().Time >= rankingRecord.Points.Last().Time);
            int firstWorseTryIndex = route.Rankingg.FindIndex(r => r.FinalResult >= finalResult);
            if (firstWorseTryIndex == -1)
                firstWorseTryIndex = route.Rankingg.Count;

            //route.Ranking.Insert(firstWorseTryIndex, new KeyValuePair<string, List<Point>>("Anon2", rankingRecord.Points.ToList()));
            var record = new RankingRecord("Anon1234", routeTimes);
            _routesRepository.InsertRankingRecord(route.Id, record);

            await Task.CompletedTask;
        }
    }
}