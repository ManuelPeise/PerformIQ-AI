namespace Data.Database
{
    // granted for all users
    public static class DefaultModuleClaims
    {
        public const string Dashboard = "dashboard";
        public const string Profile = "profile";
        
    }

    public static class ProtectedModuleClaims
    {
        public const string UserAdministration = "userAdministration";
        public const string HealthConnect = "healthConnect";
    }
}
