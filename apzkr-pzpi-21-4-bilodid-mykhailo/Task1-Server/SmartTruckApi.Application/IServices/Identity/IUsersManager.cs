using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.Identity;

namespace SmartTruckApi.Application.IServices.Identity;
public interface IUserManager
{
    Task<TokensModel> RegisterAsync(RegisterModel register, CancellationToken cancellationToken);

    Task<TokensModel> LoginAsync(LoginModel login, CancellationToken cancellationToken);

    Task<UserDto> AddToRoleAsync(string roleName, string userId, CancellationToken cancellationToken);

    Task<UserDto> RemoveFromRoleAsync(string roleName, string userId, CancellationToken cancellationToken);

    Task<UserDto> UpdateUserAsync(UserDto userDto, CancellationToken cancellationToken);

    Task<TokensModel> RefreshAccessTokenAsync(TokensModel tokensModel, CancellationToken cancellationToken);

    Task DeleteUserAsync(string userId, CancellationToken cancellationToken);
}