using AutoMapper;
using MongoDB.Bson;
using SmartTruckApi.Application.Exceptions;
using SmartTruckApi.Application.GlobalInstances;
using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Application.IServices;
using SmartTruckApi.Application.Models.CreateModels;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.OperationsModels;
using SmartTruckApi.Application.Paging;
using SmartTruckApi.Domain;
using SmartTruckApi.Domain.Enums;
using System.Reflection;

namespace SmartTruckApi.Infrastructure.Services;
public class OrdersService : IOrdersService
{
    private readonly IOrdersRepository _ordersRepository;

    private readonly ITrucksRepository _trucksRepository;

    private readonly IMapper _mapper;

    public OrdersService(IOrdersRepository ordersRepository, IMapper mapper, ITrucksRepository trucksRepository)
    {
        this._ordersRepository = ordersRepository;
        this._mapper = mapper;
        this._trucksRepository = trucksRepository;

    }

    public async Task<OrderDto> CreateOrderAsync(OrderCreateModel model, CancellationToken cancellationToken)
    {
        var orderEntity = new Order
        {
            StartPlaceX = model.StartPlaceX,
            StartPlaceY = model.StartPlaceY,
            EndPlaceX = model.EndPlaceX,
            EndPlaceY = model.EndPlaceY,
            Weight = model.Weight,
            Status = OrderStatus.Created,
            CreatedById = GlobalUser.Id.Value,
            LastModifiedById = GlobalUser.Id.Value,
            CreatedDateUtc = DateTime.UtcNow,
            LastModifiedDateUtc = DateTime.UtcNow,
        };

        var addedOrder = await this._ordersRepository.AddAsync(orderEntity, cancellationToken);

        return this._mapper.Map<OrderDto>(addedOrder);
    }

    public async Task<OrderDto> EndOrderAsync(string orderId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(orderId, out var orderObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var order = await this._ordersRepository.GetOneAsync(orderObjectId, cancellationToken);
        if (order == null)
        {
            throw new EntityNotFoundException<Order>();
        }
        var truck = new Truck();

        truck = await this._trucksRepository.GetOneAsync((ObjectId)order.TruckId, cancellationToken);
        
        if (truck == null)
        {
            throw new EntityNotFoundException<Truck>();
        }

        order.EndDateUtc = DateTime.UtcNow;
        order.Status = OrderStatus.Done;

        truck.IsAvaliable = true;

        await this._trucksRepository.UpdateTruckAsync(truck, cancellationToken);
        var updatedOrder = await this._ordersRepository.UpdateOrderAsync(order, cancellationToken);

        var result = this._mapper.Map<OrderDto>(updatedOrder);
        return result;
    }

    public async Task<OrderDto> StartOrderAsync(StartOrderModel model, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(model.Id, out var orderObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var order = await this._ordersRepository.GetOneAsync(orderObjectId, cancellationToken);
        if (order == null)
        {
            throw new EntityNotFoundException<Order>();
        }

        if (!ObjectId.TryParse(model.TruckId, out var truckObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var truck = await this._trucksRepository.GetOneAsync(truckObjectId, cancellationToken);
        if (truck == null)
        {
            throw new EntityNotFoundException<Truck>();
        }

        order.StartDateUtc = DateTime.UtcNow;
        order.Status = OrderStatus.Started;

        truck.IsAvaliable = false;

        await this._trucksRepository.UpdateTruckAsync(truck, cancellationToken);
        var updatedOrder = await this._ordersRepository.UpdateOrderAsync(order, cancellationToken);

        var result = this._mapper.Map<OrderDto>(updatedOrder);
        return result;
    }

    public async Task<PagedList<OrderDto>> GetOrdersPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _ordersRepository.GetPageAsync(pageNumber, pageSize, cancellationToken);
        var dtos = _mapper.Map<List<OrderDto>>(entities);
        var count = await _ordersRepository.GetTotalCountAsync();
        return new PagedList<OrderDto>(dtos, pageNumber, pageSize, count);
    }

    public async Task<OrderDto> GetOrderAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var orderObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var order = await this._ordersRepository.GetOneAsync(orderObjectId, cancellationToken);
        if (order == null)
        {
            throw new EntityNotFoundException<Order>();
        }

        return _mapper.Map<OrderDto>(order);
    }
}
