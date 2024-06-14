using MongoDB.Bson;
using SmartTruckApi.Domain.Common;

namespace SmartTruckApi.Domain;
public class Truck : EntityBase
{
    public string Name { get; set; }

    public ObjectId CompanyId { get; set; }

    public int Capacity { get; set; }

    public double FuelСonsumption { get; set; }

    public double? CurrentPlaceX { get; set; }

    public double? CurrentPlaceY { get; set; }

    public bool IsAvaliable { get; set; }
}
