using AutoMapper;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Domain;

namespace SmartTruckApi.Application.MappingProfiles;
public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDto>().ReverseMap();
    }
}