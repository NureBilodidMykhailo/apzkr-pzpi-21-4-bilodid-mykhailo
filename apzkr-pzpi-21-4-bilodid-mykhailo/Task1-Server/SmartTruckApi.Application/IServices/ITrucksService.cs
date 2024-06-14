using SmartTruckApi.Application.Models.CreateModels;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Paging;

namespace SmartTruckApi.Application.IServices;
public interface ITrucksService
{
    Task<PagedList<TruckDto>> GetTrucksPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<TruckDto> GetTruckAsync(string id, CancellationToken cancellationToken);

    Task<TruckDto> CreateTruckAsync(TruckCreateModel model, CancellationToken cancellationToken);

    Task<TruckDto> UpdateTruckAsync(TruckDto dto, CancellationToken cancellationToken);

    Task DeleteTruckAsync(string truckId, CancellationToken cancellationToken);

    Task RefreshCurrentPositionAsync(double currentPlaceX, double currentPlaceY, string truckId, CancellationToken cancellationToken);

    Task<OrderDto> GetBestOrderAsync(string truckId, CancellationToken cancellationToken);
}
