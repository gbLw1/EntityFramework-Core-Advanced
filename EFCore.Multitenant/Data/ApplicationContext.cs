using EFCore.Multitenant.Domain;
using EFCore.Multitenant.Provider;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Multitenant.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }

        //private readonly TenantData _tenant;

        public ApplicationContext(DbContextOptions<ApplicationContext> op /*,TenantData tenant*/) : base(op)
        {
            //_tenant = tenant;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Person>().HasData(
                new Person { Id = 1, Name = "Person 1", TenantId = "tenant-1" },
                new Person { Id = 2, Name = "Person 2", TenantId = "tenant-2" },
                new Person { Id = 3, Name = "Person 3", TenantId = "tenant-2" });

            builder.Entity<Product>().HasData(
                new Product { Id = 1, Description = "Description 1", TenantId = "tenant-1" },
                new Product { Id = 2, Description = "Description 2", TenantId = "tenant-2" },
                new Product { Id = 3, Description = "Description 3", TenantId = "tenant-2" });

            //builder.Entity<Person>().HasQueryFilter(p=>p.TenantId == _tenant.TenantId);
            //builder.Entity<Product>().HasQueryFilter(p=>p.TenantId == _tenant.TenantId);
        }
    }
}
