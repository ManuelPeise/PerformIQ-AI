using Data.Accessor;
using Data.Accessor.Interfaces;
using Data.Database.Entities;
using Data.Database.Entities.Authentication;
using Data.Database.Entities.User;
using Logic.Modules.Interfaces;
using Shared.Models.Profile;
using Shared.Models.UserManagement;

namespace Logic.Modules.UserManagement;

public class UserManagementModule(
    IApplicationUnitOfWork applicationUnitOfWork) : IUserManagementModule
{
    private const int MaxPageSize = 100;

    private readonly IApplicationUnitOfWork _applicationUnitOfWork = applicationUnitOfWork;

    // refactor: Consider using a more efficient approach for filtering and pagination, such as applying filters and pagination directly in the database query instead of in-memory filtering.
    public async Task<UserListResultModel> ListUsersAsync(UserListQueryModel queryModel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryModel);

        if (queryModel.PageNumber <= 0)
        {
            throw new ArgumentException("Page number must be greater than 0.");
        }

        if (queryModel.PageSize <= 0 || queryModel.PageSize > MaxPageSize)
        {
            throw new ArgumentException($"Page size must be between 1 and {MaxPageSize}.");
        }

        var normalizedSearchTerm = queryModel.SearchTerm?.Trim();

        var userCandidates = await _applicationUnitOfWork.Users.GetAsync(
            new DbQueryOptions<UserEntity> { AsNoTracking = true },
            cancellationToken);

        var filteredUsers = userCandidates.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
        {
            var loweredSearchTerm = normalizedSearchTerm.ToLowerInvariant();
            filteredUsers = filteredUsers.Where(x =>
                x.UserName.ToLower().Contains(loweredSearchTerm)
                || x.Email.ToLower().Contains(loweredSearchTerm));
        }

        if (queryModel.IsActive.HasValue)
        {
            filteredUsers = filteredUsers.Where(x => x.IsActive == queryModel.IsActive.Value);
        }

        var orderedUsers = filteredUsers.OrderBy(x => x.UserName).ToList();
        var totalCount = orderedUsers.Count;
        var skip = (queryModel.PageNumber - 1) * queryModel.PageSize;
        var pagedUsers = orderedUsers.Skip(skip).Take(queryModel.PageSize).ToList();

        var rolesByUserId = await GetRolesByUserIdAsync(pagedUsers.Select(x => x.Id), cancellationToken);

        return new UserListResultModel
        {
            PageNumber = queryModel.PageNumber,
            PageSize = queryModel.PageSize,
            TotalCount = totalCount,
            Users = pagedUsers.Select(user => MapToSummaryModel(user, rolesByUserId)).ToList()
        };
    }

    public async Task<UserDetailsModel> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("Valid user id is required.");
        }

        var user = await GetUserForDetailsAsync(userId, cancellationToken);
        return MapToDetailsModel(user);
    }

    public async Task<UserDetailsModel> SetUserActiveStateAsync(
        int userId,
        SetUserActiveStateRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestModel);

        if (userId <= 0)
        {
            throw new ArgumentException("Valid user id is required.");
        }

        var user = await _applicationUnitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User was not found.");

        if (user.IsActive != requestModel.IsActive)
        {
            user.IsActive = requestModel.IsActive;
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
        }

        var updatedUser = await GetUserForDetailsAsync(userId, cancellationToken);
        return MapToDetailsModel(updatedUser);
    }

    public async Task<UserDetailsModel> SetUserRolesAsync(
        int userId,
        SetUserRolesRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requestModel);

        if (userId <= 0)
        {
            throw new ArgumentException("Valid user id is required.");
        }

        var normalizedRoleNames = requestModel.RoleNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedRoleNames.Count == 0)
        {
            throw new ArgumentException("At least one role must be provided.");
        }

        var roles = await _applicationUnitOfWork.Roles.GetAsync(
            new DbQueryOptions<RoleEntity>
            {
                WhereExpression = x => normalizedRoleNames.Contains(x.Name)
            },
            cancellationToken);

        if (roles.Count != normalizedRoleNames.Count)
        {
            var missingRoles = normalizedRoleNames
                .Except(roles.Select(x => x.Name), StringComparer.OrdinalIgnoreCase);

            throw new InvalidOperationException($"The following roles were not found: {string.Join(", ", missingRoles)}.");
        }

        var user = await _applicationUnitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User was not found.");

        var existingUserRoles = await _applicationUnitOfWork.UserRoles.GetAsync(
            new DbQueryOptions<UserRoleEntity> { WhereExpression = x => x.UserId == userId },
            cancellationToken);

        var targetRoleIds = roles.Select(x => x.Id).ToHashSet();
        var currentRoleIds = existingUserRoles.Select(x => x.RoleId).ToHashSet();

        var rolesToRemove = existingUserRoles.Where(x => !targetRoleIds.Contains(x.RoleId)).ToList();
        foreach (var userRole in rolesToRemove)
        {
            await _applicationUnitOfWork.UserRoles.DeleteAsync(userRole, cancellationToken);
        }

        var roleIdsToAdd = targetRoleIds.Except(currentRoleIds);
        foreach (var roleId in roleIdsToAdd)
        {
            await _applicationUnitOfWork.UserRoles.AddAsync(new UserRoleEntity
            {
                UserId = user.Id,
                RoleId = roleId
            }, cancellationToken);
        }

        if (rolesToRemove.Count > 0 || roleIdsToAdd.Any())
        {
            await _applicationUnitOfWork.SaveChangesAsync(cancellationToken);
        }

        var updatedUser = await GetUserForDetailsAsync(userId, cancellationToken);
        return MapToDetailsModel(updatedUser);
    }

    private async Task<UserEntity> GetUserForDetailsAsync(int userId, CancellationToken cancellationToken)
    {
        var users = await _applicationUnitOfWork.Users.GetAsync(
            new DbQueryOptions<UserEntity>
            {
                AsNoTracking = true,
                Includes = { x => x.Profile! },
                WhereExpression = x => x.Id == userId
            },
            cancellationToken);

        var user = users.FirstOrDefault() ?? throw new InvalidOperationException("User was not found.");

        if (user.Profile is not null)
        {
            var profiles = await _applicationUnitOfWork.UserProfiles.GetAsync(
                new DbQueryOptions<UserProfileEntity>
                {
                    AsNoTracking = true,
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

        var rolesByUserId = await GetRolesByUserIdAsync(new[] { user.Id }, cancellationToken);
        user.UserRoles = rolesByUserId.TryGetValue(user.Id, out var roleEntities)
            ? roleEntities
            : new List<UserRoleEntity>();

        return user;
    }

    private static UserSummaryModel MapToSummaryModel(
        UserEntity user,
        IReadOnlyDictionary<int, List<UserRoleEntity>> rolesByUserId)
    {
        var userRoles = rolesByUserId.TryGetValue(user.Id, out var roles)
            ? roles
            : Enumerable.Empty<UserRoleEntity>();

        return new UserSummaryModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = userRoles
                .Select(x => x.Role.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList()
        };
    }

    private static UserDetailsModel MapToDetailsModel(UserEntity user)
    {
        var profile = user.Profile;
        var address = profile?.Address;

        return new UserDetailsModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            IsActive = user.IsActive,
            FirstName = profile?.FirstName,
            LastName = profile?.LastName,
            DateOfBirthUtc = profile is null ? null : profile.DateOfBirth,
            Address = address is null
                ? null
                : new UserAddressModel
                {
                    Street = address.Street,
                    HouseNumber = address.HouseNumber,
                    AddressLine2 = address.AddressLine2,
                    PostalCode = address.PostalCode,
                    City = address.City,
                    StateOrProvince = address.StateOrProvince,
                    CountryCode = address.CountryCode
                },
            Roles = user.UserRoles
                .Select(x => x.Role.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList()
        };
    }

    private async Task<Dictionary<int, List<UserRoleEntity>>> GetRolesByUserIdAsync(
        IEnumerable<int> userIds,
        CancellationToken cancellationToken)
    {
        var idList = userIds.Distinct().ToList();
        if (idList.Count == 0)
        {
            return new Dictionary<int, List<UserRoleEntity>>();
        }

        var userRoles = await _applicationUnitOfWork.UserRoles.GetAsync(
            new DbQueryOptions<UserRoleEntity>
            {
                AsNoTracking = true,
                Includes = { x => x.Role },
                WhereExpression = x => idList.Contains(x.UserId)
            },
            cancellationToken);

        return userRoles
            .GroupBy(x => x.UserId)
            .ToDictionary(group => group.Key, group => group.ToList());
    }
}
