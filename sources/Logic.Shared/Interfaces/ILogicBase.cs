using Data.Accessor.Interfaces;

namespace Logic.Shared.Interfaces
{
    public interface ILogicBase
    {
        IApplicationUnitOfWork ApplicationUnitOfWork { get; }
    }
}
