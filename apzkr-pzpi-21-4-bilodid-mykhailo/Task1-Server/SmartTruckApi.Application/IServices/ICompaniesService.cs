using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.Statistics;
using SmartTruckApi.Application.Paging;

namespace SmartTruckApi.Application.IServices;
public interface ICompaniesService
{
    Task<PagedList<CompanyDto>> GetCompaniesPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<CompanyDto> GetCompanyAsync(string id, CancellationToken cancellationToken);

    Task<List<TrucksStatisticsModel>> GetTrucksStatisticsAsync(string companyId, CancellationToken cancellationToken);
}
