using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database.Entities.User;
using Logic.Modules.Interfaces;
using Shared.Models.Profile;

namespace Logic.Modules.ProfileService;

public class ProfileServiceModule(
    IApplicationUnitOfWork applicationUnitOfWork) : IProfileServiceModule
{
    private readonly IApplicationUnitOfWork _applicationUnitOfWork = applicationUnitOfWork;

    public async Task<UserProfileModel> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Valid user id is required.");
        }

        var user = await GetUserWithProfileAsync(userId, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException("Authenticated user was not found.");
        }

        return MapToModel(user);
    }

    public async Task<UserProfileModel> UpsertByUserIdAsync(
        int userId,
        UpdateUserProfileRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Valid user id is required.");
        }

        if (requestModel is null)
        {
            throw new ArgumentException("Profile update payload is required.");
        }

        if (requestModel.DateOfBirthUtc.HasValue && requestModel.DateOfBirthUtc.Value > DateTime.UtcNow)
        {
            throw new ArgumentException("Date of birth cannot be in the future.");
        }

        var user = await GetUserWithProfileAsync(userId, cancellationToken);

        var profile = user.Profile;
        if (profile is null)
        {
            profile = new UserProfileEntity
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            await _applicationUnitOfWork.UserProfiles.AddAsync(profile, cancellationToken);
            user.Profile = profile;
        }
        else
        {
            profile.UserName = user.UserName;
            profile.Email = user.Email;
        }

        profile.FirstName = NormalizeOptionalText(requestModel.FirstName);
        profile.LastName = NormalizeOptionalText(requestModel.LastName);
        if (requestModel.DateOfBirthUtc.HasValue)
        {
            profile.DateOfBirth = requestModel.DateOfBirthUtc.Value;
        }

        var addressRequest = requestModel.Address;
        if (addressRequest is not null)
        {
            ValidateAddress(addressRequest);
            var address = profile.Address ?? new UserAddressEntity();

            address.Street = addressRequest.Street!.Trim();
            address.HouseNumber = addressRequest.HouseNumber!.Trim();
            address.AddressLine2 = NormalizeOptionalText(addressRequest.AddressLine2);
            address.PostalCode = addressRequest.PostalCode!.Trim();
            address.City = addressRequest.City!.Trim();
            address.StateOrProvince = NormalizeOptionalText(addressRequest.StateOrProvince);
            address.CountryCode = addressRequest.CountryCode!.Trim().ToUpperInvariant();

            profile.Address = address;
        }

        await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);

        return MapToModel(user);
    }

    private static UserProfileModel MapToModel(UserEntity user)
    {
        var profile = user.Profile;
        var addressEntity = profile?.Address;
        return new UserProfileModel
        {
            UserId = user.Id,
            UserName = profile?.UserName ?? user.UserName,
            Email = profile?.Email ?? user.Email,
            FirstName = profile?.FirstName,
            LastName = profile?.LastName,
            DateOfBirthUtc = profile is null ? null : profile.DateOfBirth,
            Address = addressEntity is null
                ? null
                : new UserAddressModel
                {
                    Street = addressEntity.Street,
                    HouseNumber = addressEntity.HouseNumber,
                    AddressLine2 = addressEntity.AddressLine2,
                    PostalCode = addressEntity.PostalCode,
                    City = addressEntity.City,
                    StateOrProvince = addressEntity.StateOrProvince,
                    CountryCode = addressEntity.CountryCode
                }
        };
    }

    private static void ValidateAddress(UpdateUserAddressRequestModel address)
    {
        if (string.IsNullOrWhiteSpace(address.Street))
        {
            throw new ArgumentException("Street is required when address is provided.");
        }

        if (string.IsNullOrWhiteSpace(address.HouseNumber))
        {
            throw new ArgumentException("House number is required when address is provided.");
        }

        if (string.IsNullOrWhiteSpace(address.PostalCode))
        {
            throw new ArgumentException("Postal code is required when address is provided.");
        }

        if (string.IsNullOrWhiteSpace(address.City))
        {
            throw new ArgumentException("City is required when address is provided.");
        }

        if (string.IsNullOrWhiteSpace(address.CountryCode))
        {
            throw new ArgumentException("Country code is required when address is provided.");
        }

        if (address.CountryCode.Trim().Length != 2)
        {
            throw new ArgumentException("Country code must have exactly 2 characters.");
        }
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private async Task<UserEntity> GetUserWithProfileAsync(int userId, CancellationToken cancellationToken)
    {
        var users = await _applicationUnitOfWork.Users.GetAsync(
            new DbQueryOptions<UserEntity>
            {
                Includes = { x => x.Profile! },
                WhereExpression = x => x.Id == userId
            },
            cancellationToken);

        var user = users.FirstOrDefault();
        if (user is null)
        {
            throw new InvalidOperationException("Authenticated user was not found.");
        }

        if (user.Profile is not null)
        {
            var profiles = await _applicationUnitOfWork.UserProfiles.GetAsync(
                new DbQueryOptions<UserProfileEntity>
                {
                    Includes = { x => x.Address! },
                    WhereExpression = x => x.Id == user.Profile.Id
                },
                cancellationToken);

            var profileWithAddress = profiles.FirstOrDefault();
            if (profileWithAddress is not null)
            {
                user.Profile = profileWithAddress;
            }
        }

        return user;
    }
}
