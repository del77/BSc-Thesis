using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Microsoft.EntityFrameworkCore;

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
        }

        public async Task<IEnumerable<Route>> GetRoutes(RoutesQuery query)
        {
            return await _context.Routes.ToListAsync();
        }
    }
}