using MongoDB.Driver;
using SmartTruckApi.Application.GlobalInstances;
using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Domain;
using SmartTruckApi.Persistance.Database;

namespace SmartTruckApi.Persistance.Repositories;
public class UsersRepository : BaseRepository<User>, IUsersRepository
{
    public UsersRepository(MongoDbContext db) : base(db, "Users") { }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<User>.Update
            .Set(u => u.Name, user.Name)
            .Set(u => u.Email, user.Email)
            .Set(u => u.Phone, user.Phone)
            .Set(u => u.Roles, user.Roles)
            .Set(u => u.CompanyId, user.CompanyId)
            .Set(u => u.PasswordHash, user.PasswordHash)
            .Set(u => u.LastModifiedDateUtc, DateTime.UtcNow)
            .Set(u => u.LastModifiedById, GlobalUser.Id);

        var options = new FindOneAndUpdateOptions<User>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await this._collection.FindOneAndUpdateAsync(
            Builders<User>.Filter.Eq(u => u.Id, user.Id) & Builders<User>.Filter.Where(x => !x.IsDeleted), updateDefinition, options, cancellationToken);
    }
}

