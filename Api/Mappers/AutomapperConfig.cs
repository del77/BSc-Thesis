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
                    cfg.CreateMap<CreateRouteDto, Route>().ReverseMap();
                    cfg.CreateMap<UserDto, User>();
                    cfg.CreateMap<PointDto, Point>()
                        .ForMember(dest => dest.Coordinates,
                            opt => opt.MapFrom(src =>
                                new NetTopologySuite.Geometries.Point(src.Longitude, src.Latitude, src.Altitude ?? 0)
                                {
                                    SRID = 4326
                                }));
                    cfg.CreateMap<Point, PointDto>()
                        .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Coordinates.Y))
                        .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Coordinates.X))
                        .ForMember(dest => dest.Altitude, opt => opt.MapFrom(src => src.Coordinates.Z));
                    cfg.CreateMap<RankingRecord, RankingRecordDto>()
                        .ForMember(dest => dest.CheckpointsTimes, opt =>
                               opt.MapFrom(src => src.CheckpointsTimes.Split(" ", StringSplitOptions.None).Select(int.Parse)))
                        .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User.Username));
                    cfg.CreateMap<RankingRecordDto, RankingRecord>()
                        .ForMember(dest => dest.CheckpointsTimes,
                            opt => opt.MapFrom(src => string.Join(" ", src.CheckpointsTimes)))
                        .ForMember(dest => dest.FinalResult, opt => opt.MapFrom(src => src.CheckpointsTimes.Last()))
                        .ForMember(dest => dest.User, opt => opt.Ignore());
                    cfg.CreateMap<RoutePropertiesDto, RouteProperties>().ReverseMap();
                })
                .CreateMapper();
    }
}