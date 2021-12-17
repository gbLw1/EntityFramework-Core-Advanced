using EFCore.UoWRepository.Domain;
using Microsoft.EntityFrameworkCore;

namespace EFCore.UoWRepository.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Colaborador> Colaboradores { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> op) : base(op)
        {

        }
    }
}
