using SmartTruckApi.Domain.Common;

namespace SmartTruckApi.Domain;
public class Company : EntityBase
{
    public string Name { get; set; }

    public string Address { get; set; }
}
