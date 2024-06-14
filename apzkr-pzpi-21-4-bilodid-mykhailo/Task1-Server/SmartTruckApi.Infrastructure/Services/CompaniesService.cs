using AutoMapper;
using MongoDB.Bson;
using SmartTruckApi.Application.Exceptions;
using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Application.IServices;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.Statistics;
using SmartTruckApi.Application.Paging;
using SmartTruckApi.Domain;
using SmartTruckApi.Domain.Enums;
using SmartTruckApi.Infrastructure.Services.Calculations;

namespace SmartTruckApi.Infrastructure.Services;
public class CompaniesService : ICompaniesService
{
    private readonly ICompaniesRepository _companiesRepository;

    private readonly ITrucksRepository _trucksRepository;

    private readonly IOrdersRepository _ordersRepository;

    private readonly IMapper _mapper;

    public CompaniesService(ICompaniesRepository companiesRepository, IMapper mapper, IOrdersRepository ordersRepository, ITrucksRepository trucksRepository)
    {
        this._mapper = mapper;
        this._companiesRepository = companiesRepository;
        this._ordersRepository = ordersRepository;
        this._trucksRepository = trucksRepository;

    }

    // Gets a paginated list of companies.
    public async Task<PagedList<CompanyDto>> GetCompaniesPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _companiesRepository.GetPageAsync(pageNumber, pageSize, cancellationToken);
        var dtos = _mapper.Map<List<CompanyDto>>(entities);
        var count = await _companiesRepository.GetTotalCountAsync();
        return new PagedList<CompanyDto>(dtos, pageNumber, pageSize, count);
    }

    // Gets a single company by its ID.
    public async Task<CompanyDto> GetCompanyAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out var companyObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var company = await this._companiesRepository.GetOneAsync(companyObjectId, cancellationToken);
        if (company == null)
        {
            throw new EntityNotFoundException<Company>();
        }

        return _mapper.Map<CompanyDto>(company);
    }

    // Gets statistics for all trucks in a specific company.
    public async Task<List<TrucksStatisticsModel>> GetTrucksStatisticsAsync(string companyId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(companyId, out var companyObjectId))
        {
            throw new InvalidDataException("Provided id is invalid.");
        }

        var company = await this._companiesRepository.GetOneAsync(companyObjectId, cancellationToken);
        if (company == null)
        {
            throw new EntityNotFoundException<Company>();
        }

        var trucks = await this._trucksRepository.GetAllAsync(t => t.CompanyId == company.Id, cancellationToken);

        var result = new List<TrucksStatisticsModel>();

        foreach(var truck in trucks)
        {
            var orders = await this._ordersRepository.GetAllAsync(o => o.TruckId == truck.Id && o.Status == OrderStatus.Done, cancellationToken);
            double fuelAmount = 0; 
            foreach(var order in orders)
            {
                fuelAmount += DistanceCalculator.CalculateDistance(order.StartPlaceX, order.StartPlaceY, order.EndPlaceX, order.EndPlaceY) / 100 * truck.FuelСonsumption;
            }

            var statModel =  new TrucksStatisticsModel
            {
                TruckName = truck.Name,
                DoneOrdersCount = orders.Count,
                SpentFuel = fuelAmount,
            };

            result.Add(statModel);
        }

        return result;
    }
}
