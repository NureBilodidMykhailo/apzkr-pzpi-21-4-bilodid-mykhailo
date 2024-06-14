using SmartTruckApi.Domain.Common;

namespace SmartTruckApi.Domain;

public class RefreshToken : EntityBase
{
    public string Token { get; set; }

    public DateTime ExpiryDateUTC { get; set; }
}
