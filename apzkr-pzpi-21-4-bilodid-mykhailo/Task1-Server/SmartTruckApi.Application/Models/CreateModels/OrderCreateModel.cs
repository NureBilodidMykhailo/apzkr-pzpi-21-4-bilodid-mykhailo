namespace SmartTruckApi.Application.Models.CreateModels;
public class OrderCreateModel
{
    public double StartPlaceX { get; set; }

    public double StartPlaceY { get; set; }

    public int Weight { get; set; }

    public double EndPlaceX { get; set; }

    public double EndPlaceY { get; set; }
}
