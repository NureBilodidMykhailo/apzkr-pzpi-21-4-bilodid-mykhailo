using SmartTruckApi.Domain.Enums;

namespace SmartTruckApi.Application.Models.Dtos;

public class OrderDto
{
    public string Id { get; set; }

    public string? TruckId { get; set; }

    public DateTime? StartDateUtc { get; set; }

    public DateTime? EndDateUtc { get; set; }

    public double StartPlaceX { get; set; }

    public double StartPlaceY { get; set; }

    public int Weight { get; set; }

    public double EndPlaceX { get; set; }

    public double EndPlaceY { get; set; }

    public OrderStatus Status { get; set; }
}
