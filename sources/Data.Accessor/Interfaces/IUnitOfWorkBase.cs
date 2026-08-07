namespace Data.Accessor.Interfaces
{
    public interface IUnitOfWorkBase
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
