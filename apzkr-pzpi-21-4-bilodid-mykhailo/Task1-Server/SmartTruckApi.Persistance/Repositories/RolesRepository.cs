using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Domain;
using SmartTruckApi.Persistance.Database;

namespace SmartTruckApi.Persistance.Repositories;
public class RolesRepository : BaseRepository<Role>, IRolesRepository
{
    public RolesRepository(MongoDbContext db) : base(db, "Roles") { }
}