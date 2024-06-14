using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTruckApi.Application.IServices;
using SmartTruckApi.Application.Models.CreateModels;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.OperationsModels;
using SmartTruckApi.Application.Paging;

namespace SmartTruckApi.Api.Controllers;
[Route("trucks")]
[ApiController]
public class TruckController : ControllerBase
{
    private readonly ITrucksService _trucksService;

    public TruckController(ITrucksService trucksService)
    {
        this._trucksService = trucksService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<TruckDto>> CreateTruckAsync([FromBody] TruckCreateModel model, CancellationToken cancellationToken)
    {
        var truck = await _trucksService.CreateTruckAsync(model, cancellationToken);
        return Ok(truck);
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshCurrentPositionAsync([FromBody] RefreshCurrentPositionModel model, CancellationToken cancellationToken)
    {
        await _trucksService.RefreshCurrentPositionAsync(model.currentPlaceX, model.currentPlaceY, model.truckId, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedList<TruckDto>>> GetTrucksPageAsync([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var trucks = await _trucksService.GetTrucksPageAsync(pageNumber, pageSize, cancellationToken);
        return Ok(trucks);
    }

    [Authorize]
    [HttpGet("{truckId}")]
    public async Task<ActionResult<OrderDto>> GetTruckAsync(string truckId, CancellationToken cancellationToken)
    {
        var truck = await _trucksService.GetTruckAsync(truckId, cancellationToken);
        return Ok(truck);
    }

    [Authorize]
    [HttpDelete("{truckId}")]
    public async Task<ActionResult> DeleteTruckAsync(string truckId, CancellationToken cancellationToken)
    {
        await _trucksService.DeleteTruckAsync(truckId, cancellationToken);
        return Ok();
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<TruckDto>> UpdateTruckAsync([FromBody] TruckDto truckDto, CancellationToken cancellationToken)
    {
        var truck = await _trucksService.UpdateTruckAsync(truckDto, cancellationToken);
        return Ok(truck);
    }

    [Authorize]
    [HttpGet("takeOrder")]
    public async Task<ActionResult<OrderDto>> GetBestOrderAsync([FromBody] string truckId, CancellationToken cancellationToken)
    {
        var order = await _trucksService.GetBestOrderAsync(truckId, cancellationToken);
        return Ok(order);
    }
}
