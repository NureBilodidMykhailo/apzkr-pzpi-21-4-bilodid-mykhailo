using AutoMapper;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Domain;

namespace SmartTruckApi.Application.MappingProfiles;
public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>().ReverseMap();
    }
}