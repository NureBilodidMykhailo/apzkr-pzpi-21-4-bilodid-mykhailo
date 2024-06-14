using SmartTruckApi.Application.Models.CreateModels;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.OperationsModels;
using SmartTruckApi.Application.Paging;

namespace SmartTruckApi.Application.IServices;

public interface IOrdersService
{
    Task<OrderDto> CreateOrderAsync(OrderCreateModel model, CancellationToken cancellationToken);

    Task<OrderDto> StartOrderAsync(StartOrderModel model, CancellationToken cancellationToken);

    Task<OrderDto> EndOrderAsync(string orderId, CancellationToken cancellationToken);

    Task<PagedList<OrderDto>> GetOrdersPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<OrderDto> GetOrderAsync(string id, CancellationToken cancellationToken);
}
