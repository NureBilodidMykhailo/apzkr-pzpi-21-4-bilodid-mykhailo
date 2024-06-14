using AutoMapper;
using MongoDB.Bson;
using SmartTruckApi.Application.Exceptions;
using SmartTruckApi.Application.GlobalInstances;
using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Application.IServices;
using SmartTruckApi.Application.Models.CreateModels;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Paging;
using SmartTruckApi.Domain;
using SmartTruckApi.Domain.Enums;
using SmartTruckApi.Infrastructure.Services.Calculations;

namespace SmartTruckApi.Infrastructure.Services;
public class TrucksService : ITrucksService
{
    private readonly ITrucksRepository _trucksRepository;

    private readonly IOrdersRepository _ordersRepository;

    private readonly IMapper _mapper;

    public TrucksService(ITrucksRepository trucksRepository, IOrdersRepository ordersRepository, IMapper mapper)
    {
        this._mapper = mapper;
        this._trucksRepository = trucksRepository;
        this._ordersRepository = ordersRepository;
    }

    public async Task<TruckDto> CreateTruckAsync(TruckCreateModel model, CancellationToken cancellationToken)
    {
        var truckEntity = new Truck
        {
            IsAvaliable = true,
            Name = model.Name,
            Capacity = model.Capacity,
            FuelСonsumption = model.FuelСonsumption,
            CompanyId = ObjectId.Parse(model.CompanyId),
            CreatedById = GlobalUser.Id.Value,
            LastModifiedById = GlobalUser.Id.Value,
            CreatedDateUtc = DateTime.UtcNow,
            LastModifiedDateUtc = DateTime.UtcNow,
        };

        var addedTruck = await this._trucksRepository.AddAsync(truckEntity, cancellationToken);

        return this._mapper.Map<TruckDto>(addedTruck);
    }

    public async Task DeleteTruckAsync(string truckId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(truckId, out var truckObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var truck = await this._trucksRepository.GetOneAsync(truckObjectId, cancellationToken);
        if (truck == null)
        {
            throw new EntityNotFoundException<Truck>();
        }

        await this._trucksRepository.DeleteAsync(truck, cancellationToken);
    }

    public async Task<OrderDto> GetBestOrderAsync(string truckId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(truckId, out var truckObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var truck = await this._trucksRepository.GetOneAsync(truckObjectId, cancellationToken);
        if (truck == null)
        {
            throw new EntityNotFoundException<Truck>();
        }

        var orders = await this._ordersRepository.GetAllAsync(o => o.Status == OrderStatus.Created && o.Weight <= truck.Capacity, cancellationToken);
        var distances = new List<double>();

        foreach (var order in orders)
        {
            distances.Add(DistanceCalculator.CalculateDistance((double)truck.CurrentPlaceX, (double)truck.CurrentPlaceY, order.StartPlaceX, order.StartPlaceY));
        }

        var min = distances.Min();
        var position = distances.FindIndex(x => x == min);

        return _mapper.Map<OrderDto>(orders[position]);
    }

    public async Task<TruckDto> GetTruckAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var truckObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var truck = await this._trucksRepository.GetOneAsync(truckObjectId, cancellationToken);
        if (truck == null)
        {
            throw new EntityNotFoundException<Truck>();
        }

        return _mapper.Map<TruckDto>(truck);
    }

    public async Task<PagedList<TruckDto>> GetTrucksPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _trucksRepository.GetPageAsync(pageNumber, pageSize, cancellationToken);
        var dtos = _mapper.Map<List<TruckDto>>(entities);
        var count = await _trucksRepository.GetTotalCountAsync();
        return new PagedList<TruckDto>(dtos, pageNumber, pageSize, count);
    }

    public async Task RefreshCurrentPositionAsync(double currentPlaceX, double currentPlaceY, string truckId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(truckId, out var truckObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var truck = await this._trucksRepository.GetOneAsync(truckObjectId, cancellationToken);
        if (truck == null)
        {
            throw new EntityNotFoundException<Truck>();
        }

        truck.CurrentPlaceX = currentPlaceX;
        truck.CurrentPlaceY = currentPlaceY;

        await this._trucksRepository.UpdateTruckAsync(truck, cancellationToken);
    }

    public async Task<TruckDto> UpdateTruckAsync(TruckDto dto, CancellationToken cancellationToken)
    {
        var truck = await this._trucksRepository.GetOneAsync(t => t.Id.ToString() == dto.Id, cancellationToken);
        if (truck == null)
        {
            throw new EntityNotFoundException<Truck>();
        }

        this._mapper.Map(dto, truck);
        var updatedTruck = await this._trucksRepository.UpdateTruckAsync(truck, cancellationToken);

        var result = this._mapper.Map<TruckDto>(updatedTruck);
        return result;
    }
}
