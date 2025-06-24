namespace WebAppDemo.Models.PermissionModels
{
    public class Module
    {
        public int ModuleId { get; set; }

        public string ModuleName { get; set; }  // Product,Category,Report
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();


    }
}
