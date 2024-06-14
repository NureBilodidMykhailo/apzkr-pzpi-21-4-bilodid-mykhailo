using Microsoft.Extensions.DependencyInjection;
using SmartTruckApi.Application.MappingProfiles;
using System.Reflection;


namespace SmartTruckApi.Application.ApplicationExtentions;
public static class MapperExtension
{
    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetAssembly(typeof(UserProfile)));

        return services;
    }
}