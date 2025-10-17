using GymWebApp.Data.Enums;

namespace GymWebApp.Data.Helpers
{
    public static class RoleHelper
    {
        public static string[] GetAllRoleNames()
        {
            return Enum.GetNames<UserRole>();
        }

        public static string GetRoleName(UserRole role)
        {
            return role.ToString();
        }

        public static UserRole? GetRoleFromName(string roleName)
        {
            if (Enum.TryParse<UserRole>(roleName, true, out var role))
            {
                return role;
            }
            return null;
        }
    }
}
