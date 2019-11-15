using System;
using System.Linq;
using Api.Dtos;
using Api.Entities;
using AutoMapper;

namespace Api.Mappers
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
            => new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CreateRouteDto, Route>();
                    cfg.CreateMap<UserDto, User>();
                    cfg.CreateMap<PointDto, Point>();
                    cfg.CreateMap<RankingRecord, RankingRecordDto>()
                        .ForMember(dest=>dest.CheckpointsTimes,opt => 
                            opt.MapFrom(src=>src.CheckpointsTimes.Split(" ", StringSplitOptions.None).Select(int.Parse)));
                    cfg.CreateMap<RankingRecordDto, RankingRecord>()
                        .ForMember(dest=>dest.CheckpointsTimes, opt => opt.MapFrom(src=>string.Join(" ", src.CheckpointsTimes)));
                    cfg.CreateMap<RoutePropertiesDto, RouteProperties>();
                })
                .CreateMapper();
    }
}