using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly IMapper _mapper;

        protected string CurrentUserId
        {
            get { return User.Claims.SingleOrDefault(u => u.Type.Contains("nameidentifier"))?.Value; }
        }

        public RoutesController(IRouteService routeService, IMapper mapper)
        {
            _routeService = routeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoutes([FromQuery]RoutesQuery query)
        {
            var routes = await _routeService.GetRoutesAsync(query);

            return Ok(_mapper.Map<IEnumerable<CreateRouteDto>>(routes));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto routeDto)
        {
            var route = _mapper.Map<Route>(routeDto);
            await _routeService.CreateRouteAsync(route, CurrentUserId);

            return Ok();
        }

        [HttpPost("{routeId}/ranking-record")]
        public async Task<IActionResult> CreateRankingRecord(Guid routeId, RankingRecordDto rankingRecordDto)
        {
            await _routeService.ProcessNewTryAsync(routeId, CurrentUserId, rankingRecordDto);

            return Ok();
        }
    }
}