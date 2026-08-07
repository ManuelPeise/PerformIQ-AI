using Data.Database.Entities.User;

namespace Logic.Authentication.Interfaces;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(UserEntity user, DateTime nowUtc);
    string CreateRefreshToken();
    string ComputeTokenHash(string token);
}
