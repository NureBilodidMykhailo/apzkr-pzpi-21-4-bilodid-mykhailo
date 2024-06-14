using AutoMapper;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Domain;

namespace SmartTruckApi.Application.MappingProfiles;
public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<Company, CompanyDto>().ReverseMap();
    }
}
