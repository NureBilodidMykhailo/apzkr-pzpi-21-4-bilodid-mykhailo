using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Domain;
using SmartTruckApi.Persistance.Database;

namespace SmartTruckApi.Persistance.Repositories;
public class CompaniesRepository : BaseRepository<Company>, ICompaniesRepository
{
    public CompaniesRepository(MongoDbContext db) : base(db, "Companies") { }
}