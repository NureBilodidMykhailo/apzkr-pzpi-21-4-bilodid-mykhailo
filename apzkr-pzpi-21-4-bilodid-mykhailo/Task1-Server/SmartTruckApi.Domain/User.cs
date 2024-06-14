using MongoDB.Bson;
using SmartTruckApi.Domain.Common;

namespace SmartTruckApi.Domain;
public class User : EntityBase
{
    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public List<Role> Roles { get; set; }

    public ObjectId CompanyId { get; set; }

}
