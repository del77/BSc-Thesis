using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;
using Api.Repositories;
using AutoMapper;

namespace Api.Services
{
    public interface IRouteService
    {
        Task<IEnumerable<Route>> GetRoutesAsync(RoutesQuery query);
        Task CreateRouteAsync(Route route, string userId);
        Task ProcessNewTryAsync(Guid routeId, string userId, RankingRecordDto rankingRecordDto);
    }
    public class RoutesService : IRouteService
    {
        private readonly IRoutesRepository _routesRepository;
        private readonly IMapper _mapper;

        public RoutesService(IRoutesRepository routesRepository, IMapper mapper)
        {
            _routesRepository = routesRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Route>> GetRoutesAsync(RoutesQuery query)
        {
            return await _routesRepository.GetRoutesAsync(query);
        }

        public async Task CreateRouteAsync(Route route, string userId)
        {
            route.Ranking.First().UserId = Guid.Parse(userId);

            await _routesRepository.CreateRouteAsync(route);
        }

        public async Task ProcessNewTryAsync(Guid routeId, string userId, RankingRecordDto rankingRecordDto)
        {
            var route = await _routesRepository.GetRouteAsync(routeId);
            
            var currentRankingRecord = route.Ranking.SingleOrDefault(rr => rr.UserId == Guid.Parse(userId));
            if (currentRankingRecord == null)
            {
                var newRankingRecord = _mapper.Map<RankingRecord>(rankingRecordDto);
                newRankingRecord.UserId = Guid.Parse(userId);
                route.Ranking.Add(newRankingRecord); 
            }
            else
            {
                if (currentRankingRecord.FinalResult < rankingRecordDto.CheckpointsTimes.Last())
                    return;

                try
                {
                    _mapper.Map(rankingRecordDto, currentRankingRecord);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            await _routesRepository.UpdateAsync();
        }
    }
}