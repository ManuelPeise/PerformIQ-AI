using Data.Database.Entities;
using System.Linq.Expressions;

namespace Data.Accessor.Interfaces;

public interface IRepositoryBase<TModel> where TModel : AEntityBase
{
    Task<IReadOnlyList<TModel>> GetAsync(DbQueryOptions<TModel>? options = null, CancellationToken cancellationToken = default);
    Task<TModel?> GetByIdAsync(int id, bool asNoTracking = false, List<Expression<Func<TModel, object>>>? includeExpressions = null, CancellationToken cancellationToken = default);
    Task<TModel> AddAsync(TModel entity, CancellationToken cancellationToken = default);
    Task<TModel> AddOrUpdateAsync(TModel entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TModel entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TModel entity, CancellationToken cancellationToken = default);
    Task DeleteRange(IEnumerable<TModel>? entities, CancellationToken cancellationToken = default);
}
