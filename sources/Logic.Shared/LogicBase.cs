using Data.Accessor.Interfaces;
using Logic.Shared.Interfaces;

namespace Logic.Shared
{
    public class LogicBase : ILogicBase
    {
        public IApplicationUnitOfWork ApplicationUnitOfWork => _applicationUnitOfWork;

        private readonly IApplicationUnitOfWork _applicationUnitOfWork;
        public LogicBase(IApplicationUnitOfWork applicationUnitOfWork)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
        }
    }
}
