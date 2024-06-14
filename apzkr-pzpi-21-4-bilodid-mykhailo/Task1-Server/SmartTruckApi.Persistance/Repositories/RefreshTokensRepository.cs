using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Domain;
using SmartTruckApi.Persistance.Database;

namespace SmartTruckApi.Persistance.Repositories;
public class RefreshTokensRepository : BaseRepository<RefreshToken>, IRefreshTokensRepository
{
    public RefreshTokensRepository(MongoDbContext db) : base(db, "RefreshTokens") { }
}
