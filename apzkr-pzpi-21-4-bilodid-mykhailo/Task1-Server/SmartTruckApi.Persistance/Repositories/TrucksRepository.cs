using MongoDB.Driver;
using SmartTruckApi.Application.GlobalInstances;
using SmartTruckApi.Application.IRepositories;
using SmartTruckApi.Application.Models.Dtos;
using SmartTruckApi.Domain;
using SmartTruckApi.Persistance.Database;

namespace SmartTruckApi.Persistance.Repositories;

public class TrucksRepository : BaseRepository<Truck>, ITrucksRepository
{
    public TrucksRepository(MongoDbContext db) : base(db, "Trucks") { }

    public async Task<Truck> UpdateTruckAsync(Truck truck, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<Truck>.Update
            .Set(t => t.CurrentPlaceX, truck.CurrentPlaceX)
            .Set(t => t.CurrentPlaceY, truck.CurrentPlaceY)
            .Set(t => t.FuelСonsumption, truck.FuelСonsumption)
            .Set(t => t.Capacity, truck.Capacity)
            .Set(t => t.CompanyId, truck.CompanyId)
            .Set(t => t.Name, truck.Name)
            .Set(t => t.IsAvaliable, truck.IsAvaliable)
            .Set(t => t.LastModifiedDateUtc, DateTime.UtcNow)
            .Set(t => t.LastModifiedById, GlobalUser.Id);

        var options = new FindOneAndUpdateOptions<Truck>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await this._collection.FindOneAndUpdateAsync(
            Builders<Truck>.Filter.Eq(t => t.Id, truck.Id) & Builders<Truck>.Filter.Where(t => !t.IsDeleted), updateDefinition, options, cancellationToken);
    }
}