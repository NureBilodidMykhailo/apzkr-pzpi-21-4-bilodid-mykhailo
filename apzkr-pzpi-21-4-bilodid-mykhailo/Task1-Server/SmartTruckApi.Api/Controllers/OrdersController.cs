using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTruckApi.Application.IServices;
using SmartTruckApi.Application.Models.CreateModels;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.OperationsModels;
using SmartTruckApi.Application.Paging;

namespace SmartTruckApi.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersService _ordersService;
    public OrdersController(IOrdersService ordersService)
    {
        this._ordersService = ordersService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<OrderDto>> CreateOrderAsync([FromBody] OrderCreateModel createModel, CancellationToken cancellationToken)
    {
        var order = await _ordersService.CreateOrderAsync(createModel, cancellationToken);
        return Ok(order);
    }

    [Authorize]
    [HttpPost("start")]
    public async Task<ActionResult<OrderDto>> StartOrderAsync([FromBody] StartOrderModel model, CancellationToken cancellationToken)
    {
        var orders = await _ordersService.StartOrderAsync(model, cancellationToken);
        return Ok(orders);
    }

    [Authorize]
    [HttpPost("end")]
    public async Task<ActionResult<OrderDto>> EndOrderAsync([FromBody] string orderId, CancellationToken cancellationToken)
    {
        var orders = await _ordersService.EndOrderAsync(orderId, cancellationToken);
        return Ok(orders);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedList<OrderDto>>> GetOrdersPageAsync([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var orders = await _ordersService.GetOrdersPageAsync(pageNumber, pageSize, cancellationToken);
        return Ok(orders);
    }

    [Authorize]
    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderDto>> GetOrderAsync(string orderId, CancellationToken cancellationToken)
    {
        var order = await _ordersService.GetOrderAsync(orderId, cancellationToken);
        return Ok(order);
    }
}
