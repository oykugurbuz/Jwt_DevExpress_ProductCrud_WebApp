namespace WebAppDemo.Models.PermissionModels
{
    public class Permission
    {
        public int PermissionId { get; set; }

        public string PermissionName { get; set; } // Create, Read, Update, Delete, Export

        public int ModuleId { get; set; } // Foreign Key 

        public Module Module { get; set; } // Navigation Property

        public ICollection<UserPermission> UserPermissions { get; set; } // Many-to-many relationship with Role
    }
}
