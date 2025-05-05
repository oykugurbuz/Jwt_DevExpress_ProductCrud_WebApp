using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
    }

}