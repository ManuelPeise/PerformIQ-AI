using Data.Accessor.Interfaces;
using Logic.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Logic.Shared
{
    public class LogicBase : ILogicBase
    {
        public IApplicationUnitOfWork ApplicationUnitOfWork => _applicationUnitOfWork;

        private readonly IApplicationUnitOfWork _applicationUnitOfWork;
        private readonly HttpContext _httpContext;
        
        public LogicBase(IApplicationUnitOfWork applicationUnitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _applicationUnitOfWork = applicationUnitOfWork;
            _httpContext = httpContextAccessor?.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public int? GetCurrentUserId()
        {
            if (_httpContext.User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = _httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }
            }

            return null;
        }

        public string GetCurrentUserName()
        {
            if (_httpContext.User.Identity?.IsAuthenticated == true)
            {
                var userNameClaim = _httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (userNameClaim != null)
                {
                    return userNameClaim.Value;
                }
            }

            return "System";
        }
    }
}
