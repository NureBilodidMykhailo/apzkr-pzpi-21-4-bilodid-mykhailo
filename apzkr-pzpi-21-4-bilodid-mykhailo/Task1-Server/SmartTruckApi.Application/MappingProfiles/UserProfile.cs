using AutoMapper;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Domain;

namespace SmartTruckApi.Application.MappingProfiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}
