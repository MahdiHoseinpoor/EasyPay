using System.Collections.Generic;

namespace EasyPay.Application.DTOs.Identity
{
    public class ManageRolePermissionsDto
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PermissionViewModel> Permissions { get; set; }
    }

    public class PermissionViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
    }
}