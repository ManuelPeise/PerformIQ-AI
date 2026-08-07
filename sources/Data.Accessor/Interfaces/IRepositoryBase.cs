using Data.Database.Entities;

namespace Data.Accessor.Interfaces;

public interface IRepositoryBase<TModel> where TModel : AEntityBase
{
    Task<IReadOnlyList<TModel>> GetAsync(DbQueryOptions<TModel>? options = null, CancellationToken cancellationToken = default);
    Task<TModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TModel> AddAsync(TModel entity, CancellationToken cancellationToken = default);
    Task<TModel> AddOrUpdateAsync(TModel entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TModel entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TModel entity, CancellationToken cancellationToken = default);
}
