namespace Shared.Models.Authentication
{
    public class CurrentUserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = [];
        public List<Permission> Permissions { get; set; } = [];
    }

    public class  Permission
    {
        public string Module { get; set; } = null!;
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
