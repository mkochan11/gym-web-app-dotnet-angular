namespace GymWebApp.Application.Common.Authorization;

public static class UserRolePolicy
{
    private static readonly string[] ManagerManageableRoles = { "Client", "Trainer", "Receptionist" };
    private static readonly string[] AdminManageableRoles = { "Client", "Trainer", "Receptionist", "Manager", "Administrator" };

    public static bool CanManageRole(string currentUserRole, string targetRole)
    {
        return currentUserRole switch
        {
            "Admin" or "Administrator" => AdminManageableRoles.Contains(targetRole),
            "Manager" => ManagerManageableRoles.Contains(targetRole),
            _ => false
        };
    }

    public static string[] GetManageableRoles(string currentUserRole)
    {
        return currentUserRole switch
        {
            "Admin" or "Administrator" => AdminManageableRoles,
            "Manager" => ManagerManageableRoles,
            _ => Array.Empty<string>()
        };
    }
}
