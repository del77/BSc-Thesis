using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Core.OpenStreetMap;
using Core.Repositories.Local;
using Core.Repositories.Web;

namespace Core.Services
{
    public class RoutesService
    {
        private readonly RoutesWebRepository _routesWebRepository;
        private readonly UserLocalRepository _userLocalRepository;
        private readonly UserData _userData;
        private const int DefaultSurfacePavement = 50;

        public RoutesService()
        {
            _routesWebRepository = new RoutesWebRepository();
            _userLocalRepository = new UserLocalRepository();
            _userData = _userLocalRepository.GetUserData();
        }

        public async Task<bool> CreateRoute(Route route)
        {
            return await _routesWebRepository.CreateRoute(route);
        }

        public void CalculateRouteDistance(Route route)
        {
            var totalDistance = 0d;

            for (int i = 0; i < route.Checkpoints.Count - 1; i++)
            {
                totalDistance += Point.HaversineKilometersDistance(route.Checkpoints[i], route.Checkpoints[i + 1]);
            }

            route.Properties.Distance = Math.Round(totalDistance, 2);
        }

        public async Task ResolveRouteParameters(Route route)
        {
            ResolveTerrainLevel(route);
            await ResolveSurface(route);
        }

        private void ResolveTerrainLevel(Route route)
        {
            const int terrainLevelDifferenceThresholdInPercentage = 10;
            const double terrainLevelDifferenceThreshold = 1.0 * terrainLevelDifferenceThresholdInPercentage / 100 + 1;
            var startAltitude = route.Checkpoints.First().Altitude;
            var finishAltitude = route.Checkpoints.Last().Altitude;

            route.Properties.TerrainLevel = TerrainLevel.Close;
            if (startAltitude != null && finishAltitude != null)
            {
                if (startAltitude > finishAltitude * terrainLevelDifferenceThreshold)
                {
                    route.Properties.TerrainLevel = TerrainLevel.Decreasing;
                }
                else if (finishAltitude > startAltitude * terrainLevelDifferenceThreshold)
                {
                    route.Properties.TerrainLevel = TerrainLevel.Increasing;
                }
            }
        }

        private async Task ResolveSurface(Route route)
        {
            var osmService = new OsmService();
            int pavedPercentage;
            try
            {
                pavedPercentage = await osmService.ResolveRouteSurfaceTypeAsync(route);
            }
            catch (Exception)
            {
                pavedPercentage = DefaultSurfacePavement;
            }

            route.Properties.PavedPercentage = pavedPercentage;
        }

        public void ProcessCurrentTry(Route route, RankingRecord currentTry)
        {
            var lastTry = route.Ranking.SingleOrDefault(rr => rr.IsMine);
            if (lastTry == null)
                return;
            if (currentTry.FinalResult < lastTry.FinalResult)
            {
                currentTry.IsMine = true;
                currentTry.IsCurrentTry = false;
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
                MarkUserRoute(route);
            }

            return routes;
        }

        private void MarkUserRoute(Route route)
        {
            var mineRankingRecord = route.Ranking.FirstOrDefault(r => r.User == _userData.Username);
            if (mineRankingRecord != null)
                mineRankingRecord.IsMine = true;
        }

        public async Task ProcessOverdueData()
        {
            var dataToSend = _userLocalRepository.GetDataToSend();

            foreach (var data in dataToSend)
            {
                var result = await _routesWebRepository.SendJsonData(data.Json, data.Uri);

                if (result)
                    _userLocalRepository.DeleteDataToSend(data.Id);
            }
        }
    }
}