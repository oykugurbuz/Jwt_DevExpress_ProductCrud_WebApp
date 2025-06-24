using System.ComponentModel.DataAnnotations;

namespace WebAppDemo.Models.PermissionModels
{
  
        public class PermissionAssignViewModel
        {
            public int SelectedUserId { get; set; }

            [Display(Name = "Kullanıcılar")]
            public List<UserDropdownItem> Users { get; set; } = new List<UserDropdownItem>();

            [Display(Name = "Modüller ve Yetkiler")]
            public List<ModuleWithPermissions> Modules { get; set; } = new List<ModuleWithPermissions>();

           
            public List<int> AssignedPermissionIds { get; set; } = new List<int>(); // Bu kullanıcıya ait hali hazırda atanmış yetkiler
        }

        public class UserDropdownItem
        {
            public int UserId { get; set; }
            public string? UserName { get; set; }
            public string? Email { get; set; }  
        }

        public class ModuleWithPermissions
        {
            public int ModuleId { get; set; }
            public string ModuleName { get; set; }
            public List<PermissionItem> Permissions { get; set; } = new List<PermissionItem>();
        }

        public class PermissionItem
        {
            public int PermissionId { get; set; }
            public string PermissionName { get; set; }
            public bool IsAssigned { get; set; }
        }
    }
