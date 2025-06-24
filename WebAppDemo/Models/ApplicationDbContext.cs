using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppDemo.Models.PermissionModels;

namespace WebAppDemo.Models
{
    public class ApplicationDbContext : DbContext
    {
        //
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<AppUserInfo> AppUserInfos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet <Category> Categories { get; set; }

        //Permission

        public DbSet <Module> Modules { get; set; }

        public DbSet <Permission> Permissions { get; set; }

        public DbSet <UserPermission> UserPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserPermission - AppUserInfo (User) ilişkisi
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)                           // UserPermission'ın "User" navigation property’si (izin alan kullanıcı)
                .WithMany(u => u.UserPermissions)                // AppUserInfo'nun "UserPermissions" koleksiyonu (1 kullanıcı birden fazla izin alabilir)
                .HasForeignKey(up => up.UserId)                   // Foreign key UserId
                .OnDelete(DeleteBehavior.Cascade);                // Kullanıcı silinirse ona bağlı UserPermission kayıtları da silinir (Cascade)

            // UserPermission - AppUserInfo (GivenByUser) ilişkisi
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.GivenByUser)                     // UserPermission'ın "GivenByUser" navigation property’si (izin veren kullanıcı)
                .WithMany()                                       // İzin veren kullanıcıda ters koleksiyon yok
                .HasForeignKey(up => up.GivenByUserId)            // Foreign key GivenByUserId
                .OnDelete(DeleteBehavior.Restrict);               // İzin veren kullanıcı silinemez (silme engellenir)

            // UserPermission - Permission ilişkisi
            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.Permission)                       // UserPermission'ın "Permission" navigation property’si
                .WithMany(p => p.UserPermissions)                  // Permission'un "UserPermissions" koleksiyonu (1 izin birden fazla kullanıcıya atanabilir)
                .HasForeignKey(up => up.PermissionId)              // Foreign key PermissionId
                .OnDelete(DeleteBehavior.Cascade);                  // Permission silinirse ona bağlı UserPermission kayıtları da silinir (Cascade)

            // Permission - Module ilişkisi
            modelBuilder.Entity<Permission>()
                .HasOne(p => p.Module)                             // Permission'ın "Module" navigation property’si (örn: Product, Category)
                .WithMany(m => m.Permissions)                       // Module'un "Permissions" koleksiyonu (1 modül birden fazla izin içerir)
                .HasForeignKey(p => p.ModuleId)                      // Foreign key ModuleId
                .OnDelete(DeleteBehavior.Cascade);                   // Module silinirse ona bağlı izinler de silinir (Cascade)
        }
    }

}