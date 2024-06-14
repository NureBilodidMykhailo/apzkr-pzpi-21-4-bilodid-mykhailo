using Microsoft.Extensions.DependencyInjection;
using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Persistance.Database;
using SmartTruckApi.Persistance.Repositories;

namespace SmartTruckApi.Persistance.PersistanceExtentions;
public static class RepositoriesExtention
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<MongoDbContext>();

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<ICompaniesRepository, CompaniesRepository>();
        services.AddScoped<ITrucksRepository, TrucksRepository>();

        return services;
    }
}