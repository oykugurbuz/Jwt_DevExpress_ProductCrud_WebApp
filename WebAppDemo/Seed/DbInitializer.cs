using System.Collections.Generic;
using System.Linq;
using WebAppDemo.Models;
using WebAppDemo.Models.PermissionModels;

namespace WebAppDemo.Seed
{
    public static class DbInitializer
    {
        public static void SeedPermissions(ApplicationDbContext context)
        {
            if (!context.Modules.Any())
            {
                var productModule = new Module { ModuleName = "Product" };
                var categoryModule = new Module { ModuleName = "Category" };
                var reportModule = new Module { ModuleName = "Report" };

                context.Modules.AddRange(productModule, categoryModule, reportModule);
                context.SaveChanges();

                var permissions = new List<Permission>
                {
                    // Product
                    new Permission { PermissionName = "Create", ModuleId = productModule.ModuleId },
                    new Permission { PermissionName = "Read", ModuleId = productModule.ModuleId },
                    new Permission { PermissionName = "Update", ModuleId = productModule.ModuleId },
                    new Permission { PermissionName = "Delete", ModuleId = productModule.ModuleId },

                    // Category
                    new Permission { PermissionName = "Create", ModuleId = categoryModule.ModuleId },
                    new Permission { PermissionName = "Read", ModuleId = categoryModule.ModuleId },
                    new Permission { PermissionName = "Update", ModuleId = categoryModule.ModuleId },
                    new Permission { PermissionName = "Delete", ModuleId = categoryModule.ModuleId },

                    // Report
                    new Permission { PermissionName = "Export", ModuleId = reportModule.ModuleId },
                };

                context.Permissions.AddRange(permissions);
                context.SaveChanges();
            }
        }
    }
}
