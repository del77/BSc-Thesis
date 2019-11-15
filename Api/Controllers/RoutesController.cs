using System.Linq;
using System.Threading.Tasks;
using Api.Entities;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;
        protected string CurrentUserId
        {
            get { return User.Claims.SingleOrDefault(u => u.Type == "NameIdentifier")?.Value; }
        }

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] Route route)
        {
            await _routeService.CreateRoute(route, CurrentUserId);

            return Ok();
        }
    }
}