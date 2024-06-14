using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SmartTruckApi.Domain;
using SmartTruckApi.Domain.Enums;
using SmartTruckApi.Infrastructure.Services.Identity;
using SmartTruckApi.Persistance.Database;

namespace SmartTruckApi.Persistance.PersistanceExtentions;
public class DbInitialaizer
{
    private readonly IMongoCollection<User> _usersCollection;

    private readonly IMongoCollection<Role> _rolesCollection;

    private readonly IMongoCollection<Company> _companiesCollection;

    private readonly IMongoCollection<Truck> _trucksCollection;

    private readonly IMongoCollection<Order> _ordersCollection;

    private readonly PasswordHasher passwordHasher;

    public DbInitialaizer(IServiceProvider services)
    {
        passwordHasher = new PasswordHasher(new Logger<PasswordHasher>(new LoggerFactory()));
        _usersCollection = services.GetService<MongoDbContext>().Db.GetCollection<User>("Users");
        _rolesCollection = services.GetService<MongoDbContext>().Db.GetCollection<Role>("Roles");
        _companiesCollection = services.GetService<MongoDbContext>().Db.GetCollection<Company>("Companies");
        _trucksCollection = services.GetService<MongoDbContext>().Db.GetCollection<Truck>("Trucks");
        _ordersCollection = services.GetService<MongoDbContext>().Db.GetCollection<Order>("Orders");
    }

    public async Task InitialaizeDb(CancellationToken cancellationToken)
    {
        await AddRoles(cancellationToken);
        await AddUsers(cancellationToken);
        await AddCompanies(cancellationToken);
        await AddTrucks(cancellationToken);
        await AddOrders(cancellationToken);
    }

    public async Task AddRoles(CancellationToken cancellationToken)
    {
        var roles = new Role[]
        {
            new Role
            {
                Id = ObjectId.Parse("6588a2f16f489bbe8f73e8e2"),
                Name = "User",
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },

            new Role
            {
                Id = ObjectId.Parse("6588a2fb7d834dad78f4672e"),
                Name = "Admin",
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },
        };
        await _rolesCollection.InsertManyAsync(roles);
    }

    public async Task AddCompanies(CancellationToken cancellationToken)
    {
        var companies = new Company[]
        {
            new Company
            {
                Id = ObjectId.Parse("665f2fa293b59ce0f1ca964e"),
                Name = "First company",
                Address = "Ukraine, Kyiv",
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },

            new Company
            {
                Id = ObjectId.Parse("665f2fabf20ee13b4841c90b"),
                Name = "Second company",
                Address = "Ukraine, Kharkiv",
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },
        };
        await _companiesCollection.InsertManyAsync(companies);
    }

    public async Task AddUsers(CancellationToken cancellationToken)
    {
        var userRole = await (await _rolesCollection.FindAsync(x => x.Name.Equals("User"))).FirstAsync();
        var adminRole = await (await _rolesCollection.FindAsync(x => x.Name.Equals("Admin"))).FirstAsync();

        var users = new User[]
        {
            new User
            {
                Id = ObjectId.Parse("6588a3049c01582e4a46384b"),
                Name = "Main Admin",
                Roles = new List<Role> {userRole, adminRole},
                Phone = "+380953326869",
                Email = "mykhailo.bilodid@nure.ua",
                PasswordHash = passwordHasher.Hash("Yuiop12345"),
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false,
                CompanyId = ObjectId.Parse("665f2fabf20ee13b4841c90b")
            },

            new User
            {
                Id = ObjectId.Parse("6588a3a41196a91b6fb30586"),
                Name = "Admin",
                Roles = new List<Role> {userRole, adminRole},
                Phone = "+380953326860",
                Email = "admin@nure.ua",
                PasswordHash = passwordHasher.Hash("Yuiop12345"),
                CreatedById = ObjectId.Parse("6588a3a41196a91b6fb30586"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3a41196a91b6fb30586"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false,
                CompanyId = ObjectId.Parse("665f2fa293b59ce0f1ca964e")
            },

            new User
            {
                Id = ObjectId.Parse("6588a3b54d09542b07fbb008"),
                Name = "User1",
                Roles = new List<Role> {userRole},
                Phone = "+380953326861",
                Email = "user1@nure.ua",
                PasswordHash = passwordHasher.Hash("Yuiop12345"),
                CreatedById = ObjectId.Parse("6588a3b54d09542b07fbb008"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3b54d09542b07fbb008"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false,
                CompanyId = ObjectId.Parse("665f2fa293b59ce0f1ca964e")
            },

            new User
            {
                Id = ObjectId.Parse("6588a3fc24555c408d83c483"),
                Name = "User2",
                Roles = new List<Role> {userRole},
                Phone = "+380953326862",
                Email = "user2@nure.ua",
                PasswordHash = passwordHasher.Hash("Yuiop12345"),
                CreatedById = ObjectId.Parse("6588a3fc24555c408d83c483"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3fc24555c408d83c483"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false,
                CompanyId = ObjectId.Parse("665f2fa293b59ce0f1ca964e")
            }
        };

        await _usersCollection.InsertManyAsync(users);
    }

    public async Task AddTrucks(CancellationToken cancellationToken)
    {
        var trucks = new Truck[]
        {
            new Truck
            {
                Id = ObjectId.Parse("665f2fa293b59ce0f1ca964e"),
                Name = "Truck 1",
                FuelСonsumption = 40,
                Capacity = 10000,
                IsAvaliable = true,
                CurrentPlaceX = 60,
                CurrentPlaceY = 60,
                CompanyId = ObjectId.Parse("665f2fa293b59ce0f1ca964e"),
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },

            new Truck
            {
                Id = ObjectId.Parse("665f32042ccd6585e237db74"),
                Name = "Truck 2",
                FuelСonsumption = 50,
                Capacity = 12000,
                IsAvaliable = true,
                CurrentPlaceX = 80,
                CurrentPlaceY = 80,
                CompanyId = ObjectId.Parse("665f2fa293b59ce0f1ca964e"),
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },

            new Truck
            {
                Id = ObjectId.Parse("665f320610e3671b3ebafbee"),
                Name = "Truck 1",
                FuelСonsumption = 30,
                Capacity = 8000,
                IsAvaliable = true,
                CurrentPlaceX = 25,
                CurrentPlaceY = 25,
                CompanyId = ObjectId.Parse("665f2fabf20ee13b4841c90b"),
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },

            new Truck
            {
                Id = ObjectId.Parse("665f320f826b6cc7069475e1"),
                Name = "Truck 2",
                FuelСonsumption = 20,
                Capacity = 5000,
                IsAvaliable = true,
                CurrentPlaceX = 44,
                CurrentPlaceY = 44,
                CompanyId = ObjectId.Parse("665f2fabf20ee13b4841c90b"),
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },
        };
        await _trucksCollection.InsertManyAsync(trucks);
    }

    public async Task AddOrders(CancellationToken cancellationToken)
    {
        var orders = new Order[]
        {
            new Order
            {
                Id = ObjectId.Parse("665f3a11d68547dcdd9625c3"),
                TruckId = ObjectId.Parse("665f2fa293b59ce0f1ca964e"),
                StartDateUtc = DateTime.UtcNow - new TimeSpan(5, 0, 0, 0),
                EndDateUtc = DateTime.UtcNow - new TimeSpan(1, 0, 0, 0),
                Weight = 1000,
                StartPlaceX = 40.1,
                StartPlaceY = 40.1,
                EndPlaceX = 20.1,
                EndPlaceY = 20.1,
                Status = OrderStatus.Done,
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },

            new Order
            {
                Id = ObjectId.Parse("665f3bf2df10d64aa4e0a65b"),
                TruckId = ObjectId.Parse("665f32042ccd6585e237db74"),
                StartDateUtc = DateTime.UtcNow - new TimeSpan(10, 0, 0, 0),
                EndDateUtc = DateTime.UtcNow - new TimeSpan(9, 0, 0, 0),
                Weight = 2000,
                StartPlaceX = 50.1,
                StartPlaceY = 50.1,
                EndPlaceX = 40.1,
                EndPlaceY = 40.1,
                Status = OrderStatus.Done,
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },

            new Order
            {
                Id = ObjectId.Parse("665f3c0d1dbd3ce37ec9a55a"),
                TruckId = ObjectId.Parse("665f320610e3671b3ebafbee"),
                StartDateUtc = DateTime.UtcNow - new TimeSpan(20, 0, 0, 0),
                EndDateUtc = DateTime.UtcNow - new TimeSpan(19, 0, 0, 0),
                Weight = 5000,
                StartPlaceX = 40.1,
                StartPlaceY = 40.1,
                EndPlaceX = 20.1,
                EndPlaceY = 20.1,
                Status = OrderStatus.Done,
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },

            new Order
            {
                Id = ObjectId.Parse("665f3c13235712ecaa748a6a"),
                TruckId = ObjectId.Parse("665f320f826b6cc7069475e1"),
                StartDateUtc = DateTime.UtcNow - new TimeSpan(100, 0, 0, 0),
                EndDateUtc = DateTime.UtcNow - new TimeSpan(90, 0, 0, 0),
                Weight = 2500,
                StartPlaceX = 60.1,
                StartPlaceY = 60.1,
                EndPlaceX = 30.1,
                EndPlaceY = 30.1,
                Status = OrderStatus.Done,
                CreatedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                CreatedDateUtc = DateTime.UtcNow,
                LastModifiedById = ObjectId.Parse("6588a3049c01582e4a46384b"),
                LastModifiedDateUtc = DateTime.UtcNow,
                IsDeleted = false
            },
        };
        await _ordersCollection.InsertManyAsync(orders);
    }
}
