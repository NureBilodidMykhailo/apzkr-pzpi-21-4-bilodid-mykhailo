using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTruckApi.Application.IServices.Identity;
using SmartTruckApi.Application.IServices;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.Identity;
using SmartTruckApi.Application.Paging;

namespace SmartTruckApi.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserManager _userManager;

    private readonly IUsersService _usersService;

    public UsersController(
        IUserManager userManager,
        IUsersService usersService)
    {
        _userManager = userManager;
        _usersService = usersService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<TokensModel>> RegisterAsync([FromBody] RegisterModel register, CancellationToken cancellationToken)
    {
        var tokens = await _userManager.RegisterAsync(register, cancellationToken);
        return Ok(tokens);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokensModel>> LoginAsync([FromBody] LoginModel login, CancellationToken cancellationToken)
    {
        var tokens = await _userManager.LoginAsync(login, cancellationToken);
        return Ok(tokens);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokensModel>> RefreshAccessTokenAsync([FromBody] TokensModel tokensModel, CancellationToken cancellationToken)
    {
        var tokens = await _userManager.RefreshAccessTokenAsync(tokensModel, cancellationToken);
        return Ok(tokens);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<UserDto>> UpdateUserAsync([FromBody] UserDto userDto, CancellationToken cancellationToken)
    {
        var user = await _userManager.UpdateUserAsync(userDto, cancellationToken);
        return Ok(user);
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> DeleteUserAsync(string userId, CancellationToken cancellationToken)
    {
        await _userManager.DeleteUserAsync(userId, cancellationToken);
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{userId}/roles/{roleName}")]
    public async Task<ActionResult<UserDto>> AddToRoleAsync(string roleName, string userId, CancellationToken cancellationToken)
    {
        var userDto = await _userManager.AddToRoleAsync(roleName, userId, cancellationToken);
        return Ok(userDto);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}/roles/{roleName}")]
    public async Task<ActionResult<UserDto>> RemoveFromRoleAsync(string roleName, string userId, CancellationToken cancellationToken)
    {
        var userDto = await _userManager.RemoveFromRoleAsync(roleName, userId, cancellationToken);
        return Ok(userDto);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<PagedList<UserDto>>> GetUsersPageAsync([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var users = await _usersService.GetUsersPageAsync(pageNumber, pageSize, cancellationToken);
        return Ok(users);
    }
}