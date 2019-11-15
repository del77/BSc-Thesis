using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Api.Repositories;

namespace Api.Services
{
    public interface IRouteService
    {
        Task<IEnumerable<Route>> GetRoutes(RoutesQuery query);
        Task CreateRoute(Route route, string userId);
    }
    public class RoutesService : IRouteService
    {
        private readonly IRoutesRepository _routesRepository;

        public RoutesService(IRoutesRepository routesRepository)
        {
            _routesRepository = routesRepository;
        }

        public async Task<IEnumerable<Route>> GetRoutes(RoutesQuery query)
        {
            throw new System.NotImplementedException();
        }

        public async Task CreateRoute(Route route, string userId)
        {
            route.Rankingg.First().UserId = Guid.Parse(userId);

            await _routesRepository.CreateRoute(route);
        }
    }
}