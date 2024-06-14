using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Infrastructure.Services.Parsing;
using System.Reflection;
using System.Text;

namespace SmartTruckApi.Api.Controllers;
[Route("export")]
[ApiController]
public class ExportController : ControllerBase
{
    private readonly IUsersRepository _usersRepository;

    private readonly IOrdersRepository _ordersRepository;

    private readonly ITrucksRepository _trucksRepository;

    private readonly ICompaniesRepository _companiesRepository;

    private readonly IRolesRepository _rolesRepository;

    private readonly IRefreshTokensRepository _refreshTokensRepository;

    public ExportController(IUsersRepository usersRepository, IOrdersRepository ordersRepository, ITrucksRepository trucksRepository, ICompaniesRepository companiesRepository, IRolesRepository rolesRepository, IRefreshTokensRepository refreshTokensRepository)
    {
        this._usersRepository = usersRepository;
        this._ordersRepository = ordersRepository;
        this._trucksRepository = trucksRepository;
        this._companiesRepository = companiesRepository;
        this._rolesRepository = rolesRepository;
        this._refreshTokensRepository = refreshTokensRepository;
    }

    [HttpGet]
    public async Task<ActionResult> ExportData(CancellationToken cancellationToken)
    {
        var users = await _usersRepository.GetAllWithDeletedAsync(cancellationToken);
        var orders = await _ordersRepository.GetAllWithDeletedAsync(cancellationToken);
        var trucks = await _trucksRepository.GetAllWithDeletedAsync(cancellationToken);
        var companies = await _companiesRepository.GetAllWithDeletedAsync(cancellationToken);
        var roles = await _rolesRepository.GetAllWithDeletedAsync(cancellationToken);
        var refreshTokens = await _refreshTokensRepository.GetAllWithDeletedAsync(cancellationToken);

        var csv = new StringBuilder();

        // Append data for each collection
        csv.AppendLine("Users");
        csv.AppendLine(CsvHelper.ConvertToCsv(users));
        csv.AppendLine();

        csv.AppendLine("Orders");
        csv.AppendLine(CsvHelper.ConvertToCsv(orders));
        csv.AppendLine();

        csv.AppendLine("Trucks");
        csv.AppendLine(CsvHelper.ConvertToCsv(trucks));
        csv.AppendLine();

        csv.AppendLine("Companies");
        csv.AppendLine(CsvHelper.ConvertToCsv(companies));
        csv.AppendLine();

        csv.AppendLine("Roles");
        csv.AppendLine(CsvHelper.ConvertToCsv(roles));
        csv.AppendLine();

        csv.AppendLine("RefreshTokens");
        csv.AppendLine(CsvHelper.ConvertToCsv(refreshTokens));

        var byteArray = Encoding.UTF8.GetBytes(csv.ToString());
        var stream = new MemoryStream(byteArray);

        return File(stream, "text/csv", "data.csv");
    }
}