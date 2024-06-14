using Microsoft.Extensions.DependencyInjection;
using SmartTruckApi.Application.IServices;
using SmartTruckApi.Application.IServices.Identity;
using SmartTruckApi.Infrastructure.Services;
using SmartTruckApi.Infrastructure.Services.Identity;

namespace SmartTruckApi.Infrastructure.InfrastructureExtentions;
public static class ServicesExtention
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokensService, TokensService>();
        services.AddScoped<ICompaniesService, CompaniesService>();
        services.AddScoped<ITrucksService, TrucksService>();
        services.AddScoped<IOrdersService, OrdersService>();

        return services;
    }
}