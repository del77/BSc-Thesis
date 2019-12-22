using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Microsoft.EntityFrameworkCore;
using Point = NetTopologySuite.Geometries.Point;

namespace Api.Repositories
{
    public interface IRoutesRepository
    {
        Task CreateRouteAsync(Route route);
        Task<IEnumerable<Route>> GetRoutesAsync(RoutesQuery query);
        Task<Route> GetRouteAsync(Guid routeId);
        Task UpdateAsync();
    }
    public class RoutesRepository : IRoutesRepository
    {
        private readonly Context _context;

        public RoutesRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateRouteAsync(Route route)
        {
            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Route>> GetRoutesAsync(RoutesQuery query)
        {
            var currentLocation = new Point(query.CurrentLongitude, query.CurrentLatitude)
            {
                SRID = 4326
            };
            var routes = _context.Routes
                .Where(r => r.Properties.PavedPercentage >= query.SurfacePavedPercentageFrom
                          && r.Properties.PavedPercentage <= query.SurfacePavedPercentageTo)
                .Where(r => r.Properties.Distance >= query.RouteLengthFrom && r.Properties.Distance <= query.RouteLengthTo)
                .Where(r => r.Checkpoints.First(cp => cp.Number == 0).Coordinates.IsWithinDistance(currentLocation, query.SearchRadiusInMeters));

            if (query.SurfaceLevel > 0)
                routes = routes.Where(r => r.Properties.HeightAboveSeaLevel == (HeightAboveSeaLevel)query.SurfaceLevel);

            return await routes
                .Include(r => r.Checkpoints)
                .Include(r => r.Properties)
                .Include(r => r.Ranking).ThenInclude(rr => rr.User)
                .ToListAsync();
        }

        public async Task<Route> GetRouteAsync(Guid routeId)
        {
            return await _context.Routes
                .Include(r => r.Checkpoints)
                .Include(r => r.Ranking).ThenInclude(rr => rr.User)
                .SingleOrDefaultAsync(r => r.Id == routeId);
        }

        public async Task UpdateAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}