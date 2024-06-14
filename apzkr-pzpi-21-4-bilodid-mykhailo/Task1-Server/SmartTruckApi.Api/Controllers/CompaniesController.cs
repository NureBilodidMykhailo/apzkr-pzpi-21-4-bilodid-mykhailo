using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTruckApi.Application.IServices;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Application.Models.Statistics;
using SmartTruckApi.Application.Paging;

namespace SmartTruckApi.Api.Controllers;

[Route("companies")]
[ApiController]
public class CompaniesController : ControllerBase
{
    private readonly ICompaniesService _companiesService;

    public CompaniesController(ICompaniesService companiesService)
    {
        this._companiesService = companiesService;
    }

    [Authorize]
    [HttpGet("statistics")]
    public async Task<ActionResult<PagedList<TrucksStatisticsModel>>> GetTrucksStatisticsAsync([FromBody] string companyId, CancellationToken cancellationToken)
    {
        var stats = await _companiesService.GetTrucksStatisticsAsync(companyId, cancellationToken);
        return Ok(stats);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedList<CompanyDto>>> GetCompaniesPageAsync([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var companies = await _companiesService.GetCompaniesPageAsync(pageNumber, pageSize, cancellationToken);
        return Ok(companies);
    }

    [Authorize]
    [HttpGet("{companyId}")]
    public async Task<ActionResult<CompanyDto>> GetCompanyAsync(string companyId, CancellationToken cancellationToken)
    {
        var company = await _companiesService.GetCompanyAsync(companyId, cancellationToken);
        return Ok(company);
    }
}
