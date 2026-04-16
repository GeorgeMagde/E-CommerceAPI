using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoobProject.Models;

namespace NoobProject.Contexts {
    public class AppDbContext : IdentityDbContext<AppUser> {

       
        public AppDbContext(DbContextOptions options) : base(options) {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder); 

            
            builder.Entity<Product>(entity => {
                entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Description).HasMaxLength(1000);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });
        }
    }
}