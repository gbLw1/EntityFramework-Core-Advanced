using DominandoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominandoEFCore.Configurations
{
    public class EstadoConfiguration : IEntityTypeConfiguration<Estado>
    {
        public void Configure(EntityTypeBuilder<Estado> builder)
        {
            builder
                .HasOne(p => p.Governador)
                .WithOne(p => p.Estado)
                .HasForeignKey<Governador>(p => p.EstadoId);

            builder
                .Navigation(p => p.Governador).AutoInclude();

            builder
                .HasMany(p => p.Cidades)
                .WithOne()
                .IsRequired();
        }
    }
}
