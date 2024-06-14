using SmartTruckApi.Domain;

namespace SmartTruckApi.Application.IRepositories;
public interface IOrdersRepository : IBaseRepository<Order>
{
    Task<Order> UpdateOrderAsync(Order order, CancellationToken cancellationToken);
}
