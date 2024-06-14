using SmartTruckApi.Domain;

namespace SmartTruckApi.Application.IRepositories;
public interface ITrucksRepository : IBaseRepository<Truck>
{
    Task<Truck> UpdateTruckAsync(Truck truck, CancellationToken cancellationToken);
}