using MongoDB.Bson;
using SmartTruckApi.Domain.Common;
using System.Linq.Expressions;

namespace SmartTruckApi.Application.IRepositories;

public interface IBaseRepository<TEntity> where TEntity : EntityBase
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task<TEntity> GetOneAsync(ObjectId id, CancellationToken cancellationToken);

    Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<List<TEntity>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<List<TEntity>> GetPageAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<int> GetTotalCountAsync();

    Task<int> GetTotalCountWithDeletedAsync();

    Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken);

    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    Task<List<TEntity>> GetAllWithDeletedAsync(CancellationToken cancellationToken);
}
