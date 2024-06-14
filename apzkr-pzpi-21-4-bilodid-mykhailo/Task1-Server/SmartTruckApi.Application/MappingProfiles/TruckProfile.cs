using AutoMapper;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Domain;

namespace SmartTruckApi.Application.MappingProfiles;

public class TruckProfile : Profile
{
    public TruckProfile()
    {
        CreateMap<Truck, TruckDto>().ReverseMap();
    }
}
