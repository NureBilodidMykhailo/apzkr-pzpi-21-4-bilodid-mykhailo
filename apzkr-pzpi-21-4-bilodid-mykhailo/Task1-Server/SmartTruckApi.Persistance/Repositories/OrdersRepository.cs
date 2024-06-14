using MongoDB.Driver;
using SmartTruckApi.Application.GlobalInstances;
using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Domain;
using SmartTruckApi.Persistance.Database;

namespace SmartTruckApi.Persistance.Repositories;
public class OrdersRepository : BaseRepository<Order>, IOrdersRepository
{
    public OrdersRepository(MongoDbContext db) : base(db, "Orders") { }

    public async Task<Order> UpdateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<Order>.Update
            .Set(o => o.EndDateUtc, order.EndDateUtc)
            .Set(o => o.StartDateUtc, order.StartDateUtc)
            .Set(o => o.Status, order.Status)
            .Set(o => o.StartPlaceX, order.StartPlaceX)
            .Set(o => o.StartPlaceY, order.StartPlaceY)
            .Set(o => o.EndPlaceX, order.EndPlaceX)
            .Set(o => o.EndPlaceY, order.EndPlaceY)
            .Set(o => o.Weight, order.Weight)
            .Set(o => o.LastModifiedDateUtc, DateTime.UtcNow)
            .Set(o => o.LastModifiedById, GlobalUser.Id);

        var options = new FindOneAndUpdateOptions<Order>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await this._collection.FindOneAndUpdateAsync(
            Builders<Order>.Filter.Eq(o => o.Id, order.Id) & Builders<Order>.Filter.Where(p => !p.IsDeleted), updateDefinition, options, cancellationToken);
    }
}
