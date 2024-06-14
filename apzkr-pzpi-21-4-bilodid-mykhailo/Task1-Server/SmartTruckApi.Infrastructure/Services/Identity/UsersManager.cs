using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using SmartTruckApi.Application.Exceptions;
using SmartTruckApi.Application.GlobalInstances;
using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Application.IServices.Identity;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.Identity;
using SmartTruckApi.Domain;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SmartTruckApi.Infrastructure.Services.Identity;
public class UserManager : IUserManager
{
    private readonly IUsersRepository _usersRepository;

    private readonly IPasswordHasher _passwordHasher;

    private readonly ITokensService _tokensService;

    private readonly IRolesRepository _rolesRepository;

    private readonly IRefreshTokensRepository _refreshTokensRepository;

    private readonly ICompaniesRepository _companiesRepository;

    private readonly IMapper _mapper;

    private readonly ILogger _logger;

    public UserManager(
        IUsersRepository usersRepository,
        IPasswordHasher passwordHasher,
        ITokensService tokensService,
        IRolesRepository rolesRepository,
        IRefreshTokensRepository refreshTokensRepository,
        IMapper mapper,
        ILogger<UserManager> logger,
        ICompaniesRepository companiesRepository)
    {
        this._usersRepository = usersRepository;
        this._logger = logger;
        this._passwordHasher = passwordHasher;
        this._tokensService = tokensService;
        this._mapper = mapper;
        this._rolesRepository = rolesRepository;
        this._refreshTokensRepository = refreshTokensRepository;
        this._companiesRepository = companiesRepository;
    }

    //Register new users
    public async Task<TokensModel> RegisterAsync(RegisterModel register, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Registering user with email: {register.Email} and phone: {register.Phone}.");

        if (!ObjectId.TryParse(register.CompanyId, out var objectId))
        {
            throw new InvalidDataException("Provided CompanyId is invalid.");
        }

        var entity = await _companiesRepository.GetOneAsync(objectId, cancellationToken);
        if (entity == null)
        {
            throw new EntityNotFoundException<Company>();
        }

        if (!register.Email.IsNullOrEmpty()) ValidateEmail(register.Email);
        if (!register.Phone.IsNullOrEmpty()) ValidatePhone(register.Phone);

        if (register.Email != null)
        {
            if (await this._usersRepository.ExistsAsync(u => u.Email == register.Email, cancellationToken))
            {
                throw new EntityAlreadyExistsException<User>("user email", register.Email);
            }
        }
        else
        {
            if (await this._usersRepository.ExistsAsync(u => u.Phone == register.Phone, cancellationToken))
            {
                throw new EntityAlreadyExistsException<User>("user phone number", register.Phone);
            }
        }

        var roleUser = await this._rolesRepository.GetOneAsync(r => r.Name == "User", cancellationToken);

        var user = new User
        {
            Name = register.Name,
            Email = register.Email,
            Phone = register.Phone,
            Roles = new List<Role> { roleUser },
            PasswordHash = this._passwordHasher.Hash(register.Password),
            CompanyId = objectId,
            LastModifiedDateUtc = DateTime.UtcNow,
            CreatedDateUtc = DateTime.UtcNow,
            CreatedById = ObjectId.Empty,
            LastModifiedById = ObjectId.Empty
        };

        await this._usersRepository.AddAsync(user, cancellationToken);

        var refreshToken = await AddRefreshToken(user.Id, cancellationToken);
        var tokens = this.GetUserTokens(user, refreshToken);

        this._logger.LogInformation($"Registered user with id: {user.Id}.");

        return tokens;
    }

    public async Task<TokensModel> LoginAsync(LoginModel login, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Logging in user with email: {login.Email} and phone: {login.Phone}.");

        var user = string.IsNullOrEmpty(login.Phone)
            ? await this._usersRepository.GetOneAsync(u => u.Email == login.Email, cancellationToken)
            : await this._usersRepository.GetOneAsync(u => u.Phone == login.Phone, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException<User>();
        }

        if (!this._passwordHasher.Check(login.Password, user.PasswordHash))
        {
            throw new InvalidDataException("Invalid password!");
        }

        var refreshToken = await AddRefreshToken(user.Id, cancellationToken);
        //Pass new tokens with refreshed data
        var tokens = this.GetUserTokens(user, refreshToken);

        this._logger.LogInformation($"Logged in user with email: {login.Email} and phone: {login.Phone}.");

        return tokens;
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(userId, out var userObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var user = await this._usersRepository.GetOneAsync(userObjectId, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException<User>();
        }

        _logger.LogInformation($"Deliting user with email: {user.Email} and phone: {user.Phone}.");

        var refreshTokenModel = await this._refreshTokensRepository
            .GetOneAsync(r =>
            r.IsDeleted == false
            && r.CreatedById.ToString() == userId
        , cancellationToken);

        await _usersRepository.DeleteAsync(user, cancellationToken);
        await _refreshTokensRepository.DeleteAsync(refreshTokenModel, cancellationToken);

        _logger.LogInformation($"Delited user with email: {user.Email} and phone: {user.Phone}.");
    }

    public async Task<TokensModel> RefreshAccessTokenAsync(TokensModel tokensModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Refreshing access token.");

        var principal = _tokensService.GetPrincipalFromExpiredToken(tokensModel.AccessToken);

        if (!ObjectId.TryParse(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var refreshTokenModel = await this._refreshTokensRepository
            .GetOneAsync(r =>
                r.Token == tokensModel.RefreshToken
                && r.CreatedById == userId
                , cancellationToken);
        if (refreshTokenModel == null || refreshTokenModel.ExpiryDateUTC < DateTime.UtcNow)
        {
            throw new SecurityTokenExpiredException();
        }

        var refreshToken = refreshTokenModel.Token;

        if (refreshTokenModel.ExpiryDateUTC.AddDays(-7) < DateTime.UtcNow)
        {
            await _refreshTokensRepository.DeleteAsync(refreshTokenModel, cancellationToken);

            var newRefreshToken = await AddRefreshToken(userId, cancellationToken);
            refreshToken = newRefreshToken.Token;
        }

        var tokens = new TokensModel
        {
            AccessToken = _tokensService.GenerateAccessToken(principal.Claims),
            RefreshToken = refreshToken
        };

        this._logger.LogInformation($"Refreshed access token.");

        return tokens;
    }

    public async Task<UserDto> AddToRoleAsync(string roleName, string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding Role: {roleName} to User with Id: {userId}.");

        var role = await this._rolesRepository.GetOneAsync(r => r.Name == roleName, cancellationToken);
        if (role == null)
        {
            throw new EntityNotFoundException<Role>();
        }

        if (!ObjectId.TryParse(userId, out var userObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var user = await this._usersRepository.GetOneAsync(userObjectId, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException<User>();
        }

        user.Roles.Add(role);
        var updatedUser = await this._usersRepository.UpdateUserAsync(user, cancellationToken);
        var userDto = this._mapper.Map<UserDto>(updatedUser);

        this._logger.LogInformation($"Added Role: {roleName} to User with Id: {userId}.");

        return userDto;
    }

    public async Task<UserDto> RemoveFromRoleAsync(string roleName, string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Removing Role: {roleName} from User with Id: {userId}.");

        var role = await this._rolesRepository.GetOneAsync(r => r.Name == roleName, cancellationToken);
        if (role == null)
        {
            throw new EntityNotFoundException<Role>();
        }

        if (!ObjectId.TryParse(userId, out var userObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var user = await this._usersRepository.GetOneAsync(userObjectId, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException<User>();
        }

        var deletedRole = user.Roles.Find(x => x.Name == role.Name);
        user.Roles.Remove(deletedRole);

        var updatedUser = await this._usersRepository.UpdateUserAsync(user, cancellationToken);
        var userDto = this._mapper.Map<UserDto>(updatedUser);

        this._logger.LogInformation($"Removed Role: {roleName} from User with Id: {userId}.");

        return userDto;
    }

    public async Task<UserDto> UpdateUserAsync(UserDto userDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating user with id: {GlobalUser.Id}.");

        var user = await this._usersRepository.GetOneAsync(x => x.Id == GlobalUser.Id, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException<User>();
        }

        this._mapper.Map(userDto, user);
        if (!string.IsNullOrEmpty(userDto.Password))
        {
            user.PasswordHash = this._passwordHasher.Hash(userDto.Password);
        }

        var updatedUser = await this._usersRepository.UpdateUserAsync(user, cancellationToken);

        var refreshToken = await AddRefreshToken(user.Id, cancellationToken);
        var tokens = this.GetUserTokens(user, refreshToken);

        var updatedUserDto = this._mapper.Map<UserDto>(updatedUser);

        this._logger.LogInformation($"Updated user with id: {GlobalUser.Id}.");

        return updatedUserDto;
    }

    private async Task<RefreshToken> AddRefreshToken(ObjectId userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding new refresh token for user with Id : {userId}.");

        var refreshToken = new RefreshToken
        {
            Token = _tokensService.GenerateRefreshToken(),
            ExpiryDateUTC = DateTime.UtcNow.AddDays(30),
            CreatedById = userId,
            CreatedDateUtc = DateTime.UtcNow
        };

        await this._refreshTokensRepository.AddAsync(refreshToken, cancellationToken);

        this._logger.LogInformation($"Added new refresh token.");

        return refreshToken;
    }

    private TokensModel GetUserTokens(User user, RefreshToken refreshToken)
    {
        var claims = this.GetClaims(user);
        var accessToken = this._tokensService.GenerateAccessToken(claims);

        this._logger.LogInformation($"Returned new access and refresh tokens.");

        return new TokensModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
        };
    }

    private IEnumerable<Claim> GetClaims(User user)
    {
        var claims = new List<Claim>()
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Email, user.Email ?? string.Empty),
                new (ClaimTypes.MobilePhone, user.Phone ?? string.Empty),
            };

        foreach (var role in user.Roles)
        {
            claims.Add(new(ClaimTypes.Role, role.Name));
        }

        this._logger.LogInformation($"Returned claims for User with Id: {user.Id}.");

        return claims;
    }

    private void ValidateEmail(string email)
    {
        string regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        if (!Regex.IsMatch(email, regex))
        {
            throw new InvalidEmailException(email);
        }
    }

    private void ValidatePhone(string phone)
    {
        string regex = @"^\+[0-9]{1,15}$";

        if (!Regex.IsMatch(phone, regex))
        {
            throw new InvalidPhoneNumberException(phone);
        }
    }
}