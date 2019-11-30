using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Algorithm.Distance;
using Point = NetTopologySuite.Geometries.Point;

namespace Api.Repositories
{
    public interface IRoutesRepository
    {
        Task CreateRoute(Route route);
        Task<IEnumerable<Route>> GetRoutes(RoutesQuery query);
    }
    public class RoutesRepository : IRoutesRepository
    {
        private readonly Context _context;

        public RoutesRepository(Context context)
        {
            _context = context;
        }

        public async Task CreateRoute(Route route)
        {
            await _context.Routes.AddAsync(route);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Route>> GetRoutes(RoutesQuery query)
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
    }
}