using MongoDB.Bson;

namespace SmartTruckApi.Application.Models.Dtos;

public class TruckDto
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string CompanyId { get; set; }

    public int Capacity { get; set; }

    public double FuelСonsumption { get; set; }

    public double? CurrentPlaceX { get; set; }

    public double? CurrentPlaceY { get; set; }

    public bool IsAvaliable { get; set; }
}
