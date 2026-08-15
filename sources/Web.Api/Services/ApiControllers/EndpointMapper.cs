using Web.Api.Services.ApiControllers.Authentication;
using Web.Api.Services.ApiControllers.UserService;

namespace Web.Api.Services.ApiControllers
{
    internal static class EndpointMapper
    {
        internal static void MapEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            AuthenticationEndpointMapper.MapAuthEndpoints(endpointRouteBuilder);
            UserServiceEndpointMapper.MapUserServiceEndpoints(endpointRouteBuilder);
        }
    }
}
