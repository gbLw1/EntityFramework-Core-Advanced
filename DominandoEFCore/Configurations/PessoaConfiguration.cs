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
    #region [TPH → Tipo por Hierarquia]

    //public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    //{
    //    public void Configure(EntityTypeBuilder<Pessoa> builder)
    //    {
    //        builder
    //            .ToTable("Pessoas")
    //            .HasDiscriminator<int>("TipoPessoa")
    //            .HasValue<Pessoa>(3)
    //            .HasValue<Instrutor>(6)
    //            .HasValue<Aluno>(99);
    //    }
    //}

    #endregion

    #region [TPT → Tipo por Tipo ↓]

    public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    {
        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder
                .ToTable("Pessoas");
        }
    }
    public class InstrutorConfiguration : IEntityTypeConfiguration<Instrutor>
    {
        public void Configure(EntityTypeBuilder<Instrutor> builder)
        {
            builder
                .ToTable("Instrutores");
        }
    }
    public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
    {
        public void Configure(EntityTypeBuilder<Aluno> builder)
        {
            builder
                .ToTable("Alunos");
        }
    }

    #endregion
}
