using Shared.Models.Authentication;

namespace Shared.Models.UserManagement
{
    public class UserDataExportModel
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? DateOfBirthUtc { get; set; }
        public bool IsActive { get; set; }
        public bool IsMarkedAsDeleted { get; set; } = false;
        public string? IsMarkedAdDeletedBy { get; set; } = null;
        public string? IsMarkedAdDeletedAt { get; set; } = null;
        public List<string> Roles { get; set; } = [];
        public List<Permission> Permissions { get; set; } = [];
    }
}