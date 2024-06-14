using SmartTruckApi.Domain;

namespace SmartTruckApi.Application.IRepositories;
public interface IUsersRepository : IBaseRepository<User>
{
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken);
}
