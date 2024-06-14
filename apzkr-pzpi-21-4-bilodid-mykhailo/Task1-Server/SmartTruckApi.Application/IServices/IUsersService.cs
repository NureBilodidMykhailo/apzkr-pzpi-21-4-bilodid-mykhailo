using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Paging;

namespace SmartTruckApi.Application.IServices;
public interface IUsersService
{
    Task<PagedList<UserDto>> GetUsersPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<UserDto> GetUserAsync(string id, CancellationToken cancellationToken);
}