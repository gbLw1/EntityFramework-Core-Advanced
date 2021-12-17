using DominandoEFCore.Domain;
using DominandoEFCore.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DominandoEFCore.UDFs;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace DominandoEFCore.Data
{
    public class ApplicationContext : DbContext
    {
        #region [+ Entidades DbSet]

        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Documento> Documentos { get; set; }

        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Instrutor> Instrutores { get; set; }
        public DbSet<Aluno> Alunos { get; set; }

        public DbSet<Atributo> Atributos { get; set; }

        public DbSet<Funcao> Funcoes { get; set; }

        public DbSet<Livro> Livros { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#warning Sensitive content: sql connection string
            const string connectionString = "Server=#;Database=#;User Id=#;Password=#;pooling=true";

            optionsBuilder
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region [SQL Collate → CS (Case Sensitive) + AS (Accent Sensitive) ↓]
            /*
            builder.UseCollation("SQL_Latin1_General_CP1_CI_AI");
            builder.Entity<Departamento>().Property(p => p.Descricao).UseCollation("SQL_Latin1_General_CP1_CS_AS");
            */
            #endregion

            #region [SQL Sequence ↓]
            /*
            builder
                .HasSequence<int>("MinhaSequencia", "sequencias")
                .StartsAt(1)
                .IncrementsBy(2)
                .HasMin(1)
                .HasMax(10)
                .IsCyclic();

            builder.Entity<Departamento>().Property(p => p.Id).HasDefaultValueSql("NEXT VALUE FOR sequencias.MinhaSequencia");
            */
            #endregion

            #region [SQL Index]
            /*
            builder.Entity<Departamento>()
                .HasIndex(p => new { p.Descricao, p.Ativo })
                .HasDatabaseName("idx_meu_indice_composto")
                .HasFilter("Descricao IS NOT NULL")
                .HasFillFactor(80)
                .IsUnique();
            */
            #endregion

            #region [Propagação de dados]
            /*
            builder.Entity<Estado>().HasData(new[]
            {
                new Estado() { Id = 1, Nome = "São Paulo" },
                new Estado() { Id = 2, Nome = "Sergipe" }
            });
            */
            #endregion

            #region [Esquemas]
            /*
            builder.HasDefaultSchema("cadastros");
            builder.Entity<Estado>().ToTable("Estados", "SegundoEsquema");
            */
            #endregion

            #region [Shadow Properties]
            //builder.Entity<Departamento>().Property<DateTime>("UltimaAtualizacao");
            #endregion

            #region [Owned Types]

            //builder.Entity<Cliente>(p =>
            //{
            //    p.OwnsOne(x => x.Endereco, end =>
            //    {
            //        end.ToTable("Enderecos");
            //    });
            //});

            #endregion

            #region [EntityTypeConfiguration]

            //builder.ApplyConfiguration(new ClienteConfiguration());
            //builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            #endregion

            #region [Função Property]

            //builder
            //    .Entity<Funcao>(p =>
            //    {
            //        p.Property<string>("PropriedadeSombra")
            //            .HasColumnType("VARCHAR(100)")
            //            .HasDefaultValueSql("'Teste'");
            //    });

            #endregion

            #region [Mapeamento das funções]

            //// Registrando funções via Data Annotations ↓
            ////MinhasFuncoes.Registrar(builder);

            //// Registrando funções via Fluent API ↓
            //builder
            //    .HasDbFunction(_minhaFuncao)
            //    .HasName("LEFT")
            //    .IsBuiltIn();

            //builder
            //    .HasDbFunction(_letrasMaiusculas)
            //    .HasName("ConverterParaLetrasMaiusculas")
            //    .HasSchema("dbo");

            //builder
            //    .HasDbFunction(_dateDiff)
            //    .HasName("DATEDIFF")
            //    .HasTranslation(p =>
            //    {
            //        var argumentos = p.ToList();

            //        var constante = (SqlConstantExpression)argumentos[0];
            //        argumentos[0] = new SqlFragmentExpression(constante.Value.ToString());

            //        return new SqlFunctionExpression("DATEDIFF", argumentos, false, new[]{false, false, false}, typeof(int), null);
            //    })
            //    .IsBuiltIn();

            #endregion
        }

        #region [Módulo - UDFs]

        //// Métodos Extraído do mapeamento das funções via Fluent API
        //private static MethodInfo _minhaFuncao = typeof(MinhasFuncoes)
        //                .GetRuntimeMethod("Left", new[] { typeof(string), typeof(int) });

        //private static MethodInfo _letrasMaiusculas = typeof(MinhasFuncoes)
        //                .GetRuntimeMethod(nameof(MinhasFuncoes.LetrasMaiusculas), new[] { typeof(string)});

        //private static MethodInfo _dateDiff = typeof(MinhasFuncoes)
        //    .GetRuntimeMethod(nameof(MinhasFuncoes.DateDiff), 
        //    new[] { typeof(string), typeof(DateTime), typeof(DateTime) });

        #endregion

        #region [Módulo - Infraestrutura]

        /*
        
        //private readonly StreamWriter _writer = new StreamWriter(@"C:\...\yourPath\...\myLog.txt", append: true);
        
        //OnConfiguring ↓ 
            //optionsBuilder
                //.UseSqlServer(connectionString,
                //                o => o
                //                .MaxBatchSize(100) // default: 42
                //                .CommandTimeout(5)
                //                .EnableRetryOnFailure(4, TimeSpan.FromSeconds(10), null))
                //.LogTo(_writer.WriteLine, LogLevel.Information);


        public override void Dispose()
        {
            base.Dispose();
            _writer.Dispose();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<Departamento>().HasQueryFilter(p => !p.Excluido); // ← Filtro global
        }
        */

        #endregion

        #region [Módulo - Interceptação]

        /*
        optionsBuilder
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .AddInterceptors(new Interceptadores.InterceptadoresDeComandos())
                .AddInterceptors(new Interceptadores.InterceptadorDeConexao())
                .AddInterceptors(new Interceptadores.InterceptadorPersistencia());
         */

        #endregion
    }
}
