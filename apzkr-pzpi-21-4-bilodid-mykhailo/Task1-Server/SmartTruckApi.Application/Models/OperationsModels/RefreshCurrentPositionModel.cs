namespace SmartTruckApi.Application.Models.OperationsModels;
public class RefreshCurrentPositionModel
{
    public string truckId { get; set; }

    public double currentPlaceX { get; set; }

    public double currentPlaceY { get; set; }
}
