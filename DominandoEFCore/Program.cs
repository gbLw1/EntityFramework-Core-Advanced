using DominandoEFCore.Diagnostics;
using DominandoEFCore.Domain;
using DominandoEFCore.UDFs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Diagnostics;

namespace DominandoEFCore
{
    class Program
    {
        // Testes direto pelo SVM ↓

        static void Main(string[] args)
        {
            #region [Gerenciar o estado da Conexão]
            ////warmup
            //new Data.ApplicationContext().Departamentos.AsNoTracking().Any();
            //_count = 0;
            //GerenciarEstadoDaConexao(false);
            //_count = 0;
            //GerenciarEstadoDaConexao(true);
            #endregion


        }

        // Módulos ↓

        #region [+ EF Database]

        static void EnsureCreatedAndDeleted()
        {
            using var db = new Data.ApplicationContext();
            //db.Database.EnsureCreated();
            //db.Database.EnsureDeleted();
        }

        static void GapToEnsureCreated()
        {
            using var db1 = new Data.ApplicationContext();
            //using var db2 = new Data.ApplicationContextCidade();

            //db1.Database.EnsureCreated();
            //db2.Database.EnsureCreated();

            //var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            //databaseCreator.CreateTables();
        }

        static void HealthCheckBancoDeDados()
        {
            using var db = new Data.ApplicationContext();
            var canConnect = db.Database.CanConnect();

            if (canConnect)
                Console.WriteLine("Conectado.");
            else
                Console.WriteLine("Desconectado");

            #region [Método antigo de teste de conexão]
            //try
            //{
            //    // opção1 de teste de conexão com o banco
            //    var connection = db.Database.GetDbConnection();
            //    connection.Open();

            //    // opção2 de teste de conexão com o banco
            //    db.Departamentos.Any();

            //    Console.WriteLine("Conectado.");
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("Desconectado.");
            //}
            #endregion
        }

        static int _count;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoConexao)
        {
            using var db = new Data.ApplicationContext();
            var time = System.Diagnostics.Stopwatch.StartNew();
            var conexao = db.Database.GetDbConnection();

            conexao.StateChange += (_, __) => _count++;

            if (gerenciarEstadoConexao)
                conexao.Open();

            for (int i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            }

            time.Stop();
            Console.WriteLine($"Tempo = {time.Elapsed.ToString()}, {gerenciarEstadoConexao}, Contador: {_count}");
        }

        static void ExecuteSQL()
        {
            using var db = new Data.ApplicationContext();

            // Opção 1
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            // Opção 2
            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("update departamentos set descricao={0} where id=1", descricao);

            // Opção 3
            db.Database.ExecuteSqlInterpolated($"update departamentos set descricao={descricao} where id=1");
        }

        static void SqlInjection()
        {
            using var db = new Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.AddRange(
                new Domain.Departamento
                {
                    Descricao = "Departamento 01"
                },
                new Domain.Departamento
                {
                    Descricao = "Departamento 02"
                });
            db.SaveChanges();

            // Método errado ↓ (Aplicação dos valores diretamente na instrução Sql)
            //var descricao = "Teste ' or 1='1";
            //db.Database.ExecuteSqlRaw($"update departamentos set descricao='AtaqueSqlInjection' where descricao='{descricao}'");

            // Método correto e mais seguro para Fazer instruções Sql (SqlRaw && SqlInterpolated)
            var descricao = "Departamento 01";
            db.Database.ExecuteSqlRaw("update departamentos set descricao='DepartamentoAlterado' where descricao={0}", descricao);

            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id: {departamento.Id}, Descrição: {departamento.Descricao}");
            }
        }

        static void MigracoesPendentes()
        {
            using var db = new Data.ApplicationContext();

            var migracoesPendentes = db.Database.GetPendingMigrations();

            Console.WriteLine($"Total: {migracoesPendentes.Count()}");

            foreach (var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void AplicarMigracoesEmTempoDeExecucao()
        {
            using var db = new Data.ApplicationContext();
            db.Database.Migrate();
        }

        static void TodasMigracoes()
        {
            using var db = new Data.ApplicationContext();

            var migracoes = db.Database.GetMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void MigracoesJaAplicadas()
        {
            using var db = new Data.ApplicationContext();

            var migracoes = db.Database.GetAppliedMigrations();

            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void ScriptDoBancoDeDados()
        {
            using var db = new Data.ApplicationContext();
            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }

        #endregion

        #region [+ Tipos de Carregamento]

        static void SetupTiposCarregamentos(Data.ApplicationContext db)
        {
            if (!db.Departamentos.Any())
            {
                db.Departamentos.AddRange(
                    new Domain.Departamento
                    {
                        Descricao = "Departamento 01",
                        Funcionarios = new List<Domain.Funcionario>
                        {
                            new Domain.Funcionario
                            {
                                Nome = "Gabriel",
                                CPF = "99999999911",
                                RG = "2100062"
                            }
                        }
                    },
                    new Domain.Departamento
                    {
                        Descricao = "Departamento 02",
                        Funcionarios = new List<Domain.Funcionario>
                        {
                            new Domain.Funcionario
                            {
                                Nome = "Rafael Almeida",
                                CPF = "88888888811",
                                RG = "3100062"
                            },
                            new Domain.Funcionario
                            {
                                Nome = "Eduardo Pires",
                                CPF = "77777777711",
                                RG = "1100062"
                            }
                        }
                    });
                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        static void CarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .Include(p => p.Funcionarios);

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine("\tNenhum funcionario encontrado!");
                }
            }
        }

        static void CarregamentoExplicito()
        {
            using var db = new Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .ToList();

            foreach (var departamento in departamentos)
            {
                if (departamento.Id == 2)
                {
                    //db.Entry(departamento).Collection(p => p.Funcionarios).Load();
                    db.Entry(departamento).Collection(p => p.Funcionarios).Query().Where(p => p.Id > 2).ToList();
                }

                Console.WriteLine("------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"Funcionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine("\tNenhum funcionario encontrado!");
                }
            }
        }

        static void CarregamentoLento()
        {
            using var db = new Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                        Console.WriteLine($"Funcionario: {funcionario.Nome}");
                }
                else
                    Console.WriteLine("\tNenhum funcionario encontrado!");
            }
        }

        #endregion

        #region [+ Consultas]

        static void Setup(Data.ApplicationContext db)
        {
            if (db.Database.EnsureCreated())
            {
                db.Departamentos.AddRange(
                    new Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 01",
                        Funcionarios = new List<Domain.Funcionario>
                        {
                            new Domain.Funcionario
                            {
                                Nome = "Gabriel",
                                CPF = "99999999911",
                                RG = "2100062"
                            }
                        },
                        Excluido = true
                    },
                    new Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 02",
                        Funcionarios = new List<Domain.Funcionario>
                        {
                            new Domain.Funcionario
                            {
                                Nome = "Rafael Almeida",
                                CPF = "88888888811",
                                RG = "3100062"
                            },
                            new Domain.Funcionario
                            {
                                Nome = "Eduardo Pires",
                                CPF = "77777777711",
                                RG = "1100062"
                            }
                        }
                    });

                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }

        static void FiltroGlobal()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \tExcluido: {departamento.Excluido}");
            }
        }

        static void IgnoreFiltroGlobal()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao} \tExcluido: {departamento.Excluido}");
            }
        }

        static void ConsultaProjetada()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .Where(p => p.Id > 0)
                .Select(p => new { p.Descricao, Funcionarios = p.Funcionarios.Select(f => f.Nome) })
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\t Nome: {funcionario}");
                }
            }
        }

        static void ConsultaParametrizada()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var id = 0;
            var departamentos = db.Departamentos
                .FromSqlRaw("SELECT * FROM Departamentos WHERE Id>{0}", id)
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void ConsultaInterpolada()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var id = 0;
            var departamentos = db.Departamentos
                .FromSqlInterpolated($"SELECT * FROM Departamentos WHERE Id>{id}")
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void ConsultaComTAG()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .TagWith(@"Estou Enviando um comentário para o servidor

                    Segundo comentário
                    Terceiro comentário")
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }

        static void Consulta1NN1()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var funcionarios = db.Funcionarios
                .Include(p => p.Departamento)
                .ToList();

            foreach (var funcionario in funcionarios)
            {
                Console.WriteLine($"Funcionario: {funcionario.Nome} / Descrição Dep: {funcionario.Departamento.Descricao}");
            }

            //var departamentos = db.Departamentos
            //    .Include(p => p.Funcionarios)
            //    .ToList();

            //foreach (var departamento in departamentos)
            //{
            //    Console.WriteLine($"Descrição: {departamento.Descricao}");

            //    foreach (var funcionario in departamento.Funcionarios)
            //    {
            //        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
            //    }
            //}
        }

        static void DivisaoDeConsulta()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .Include(p => p.Funcionarios)
                .Where(p => p.Id > 3)
                .AsSplitQuery()
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                }
            }
        }

        #endregion

        #region [+ Stored Procedures]

        static void CriarStoredProcedureDeConsulta()
        {
            var criarProcedure = @"
            CREATE OR ALTER PROCEDURE GetDepartamentos
                @Descricao VARCHAR(50)
            AS
            BEGIN
                SELECT * FROM Departamentos WHERE Descricao LIKE @Descricao + '%'
            END
            ";

            using var db = new Data.ApplicationContext();

            db.Database.ExecuteSqlRaw(criarProcedure);
        }

        static void ConsultaViaProcedure()
        {
            using var db = new Data.ApplicationContext();

            var departamentos = db.Departamentos
                .FromSqlRaw("EXECUTE GetDepartamentos @p0", "Departamento")
                .ToList();
            foreach (var departamento in departamentos)
            {
                Console.WriteLine(departamento.Descricao);
            }
        }

        #endregion

        #region [+ Infraestrutura]

        static void ExecutarEstrategiaResiliencia()
        {
            using var db = new Data.ApplicationContext();

            var strategy = db.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var transaction = db.Database.BeginTransaction();

                db.Departamentos.Add(new Domain.Departamento { Descricao = "Departamento Transacao" });
                db.SaveChanges();

                transaction.Commit();
            });
        }

        #endregion

        #region [+ Modelo de dados]

        static void Collations()
        {
            using var db = new Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        static void PropagarDados()
        {
            using var db = new Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }

        static void Esquema()
        {
            using var db = new Data.ApplicationContext();

            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        static void TrabalhandoComPropriedadeDeSombra()
        {
            using var db = new Data.ApplicationContext();
            /*
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var departamento = new Departamento
            {
                Descricao = "Departamento Propriedade de Sombra"
            };

            db.Departamentos.Add(departamento);

            db.Entry(departamento).Property("UltimaAtualizacao").CurrentValue = DateTime.Now;

            db.SaveChanges();
            */

            var departamentos = db.Departamentos.Where(p => EF.Property<DateTime>(p, "UltimaAtualizacao") < DateTime.Now).ToArray();
        }

        static void TiposDePropriedades()
        {
            using var db = new Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var cliente = new Cliente
            {
                Nome = "Fulano de tal",
                Telefone = "(11) 98888-8888",
                Endereco = new Endereco { Bairro = "Centro", Cidade = "São Paulo" }
            };

            db.Clientes.Add(cliente);
            db.SaveChanges();

            var clientes = db.Clientes.AsNoTracking().ToList();
            var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };

            clientes.ForEach(cliente =>
            {
                var json = System.Text.Json.JsonSerializer.Serialize(cliente, options);

                Console.WriteLine(json);
            });
        }

        static void Relacionamento1Para1()
        {
            using var db = new Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var estado = new Estado
            {
                Nome = "Sergipe",
                Governador = new Governador { Nome = "Rafael Almeida" }
            };

            db.Estados.Add(estado);
            db.SaveChanges();

            //var estados = db.Estados.Include(p=>p.Governador).AsNoTracking().ToList();
            var estados = db.Estados.AsNoTracking().ToList();

            estados.ForEach(estado =>
            {
                Console.WriteLine($"Estado: {estado.Nome}, Governador: {estado.Governador.Nome}");
            });
        }

        static void Relacionamento1ParaMuitos()
        {
            using (var db = new Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var estado = new Estado
                {
                    Nome = "Sergipe",
                    Governador = new Governador { Nome = "Rafael Almeida" }
                };

                estado.Cidades.Add(new Cidade { Nome = "Itabaiana" });

                db.Estados.Add(estado);
                db.SaveChanges();

            }

            using (var db = new Data.ApplicationContext())
            {
                var estados = db.Estados.ToList();

                estados[0].Cidades.Add(new Cidade { Nome = "Aracaju" });

                db.SaveChanges();

                foreach (var est in db.Estados.Include(p => p.Cidades).AsNoTracking())
                {
                    Console.WriteLine($"Estado: {est.Nome}, Governador {est.Governador.Nome}");

                    foreach (var cidade in est.Cidades)
                    {
                        Console.WriteLine($"\tCidade: {cidade.Nome}");
                    }
                }
            }

        }

        static void CampoDeApoio()
        {
            using (var db = new Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var documento = new Documento();
                documento.SetCPF("12345678911");

                db.Documentos.Add(documento);
                db.SaveChanges();

                foreach (var doc in db.Documentos.AsNoTracking())
                {
                    Console.WriteLine($"CPF → {doc.GetCPF()}");
                }
            }
        }

        // Tabela por Hierarquia + Tabela por Tipo
        static void ExemploTPH()
        {
            using (var db = new Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var pessoa = new Pessoa { Nome = "Fulano de Tal" };
                var instrutor = new Instrutor { Nome = "Rafael Almeida", Tecnologia = ".NET", Desde = DateTime.Now };
                var aluno = new Aluno { Nome = "Maria Thysbe", Idade = 31, DataContrato = DateTime.Now.AddDays(-1) };

                db.AddRange(pessoa, instrutor, aluno);
                db.SaveChanges();

                var pessoas = db.Pessoas.AsNoTracking().ToArray();
                var instrutores = db.Instrutores.AsNoTracking().ToArray();
                //var alunos = db.Alunos.AsNoTracking().ToArray();
                var alunos = db.Pessoas.OfType<Aluno>().AsNoTracking().ToArray();

                Console.WriteLine("\n=====//=====\n Pessoas\n=====//=====");
                foreach (var p in pessoas)
                {
                    Console.WriteLine($"Id: {p.Id} → {p.Nome}");
                }

                Console.WriteLine("\n=====//=====\n Instrutores\n=====//=====");
                foreach (var p in instrutores)
                {
                    Console.WriteLine($"Id: {p.Id} → {p.Nome}, Tecnologia: {p.Tecnologia}, Desde {p.Desde}");
                }

                Console.WriteLine("\n=====//=====\n Alunos\n=====//=====");
                foreach (var p in alunos)
                {
                    Console.WriteLine($"Id: {p.Id} → {p.Nome}, Idade {p.Idade}, Data do Contrato: {p.DataContrato}");
                }
            }
        }

        #endregion

        #region [+ Data Annotations]

        static void Atributos()
        {
            using (var db = new Data.ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);
            }
        }

        #endregion

        #region [+ EF Functions]

        static void ApagarCriarBD()
        {
            using var db = new Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Funcoes.AddRange(
            new Funcao
            {
                Data1 = DateTime.Now.AddDays(2),
                Data2 = "2021-01-01",
                Descricao1 = "Bala 1 ",
                Descricao2 = "Bala 1 "
            },
            new Funcao
            {
                Data1 = DateTime.Now.AddDays(1),
                Data2 = "XX21-01-01",
                Descricao1 = "Bola 2 ",
                Descricao2 = "Bola 2 "
            },
            new Funcao
            {
                Data1 = DateTime.Now.AddDays(1),
                Data2 = "XX21-01-01",
                Descricao1 = "Tela",
                Descricao2 = "Tela"
            });

            db.SaveChanges();
        }

        static void FuncoesDeData()
        {
            ApagarCriarBD();

            using (var db = new Data.ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();

                Console.WriteLine(script);

                var dados = db.Funcoes.AsNoTracking().Select(p =>
                new
                {
                    Dias = EF.Functions.DateDiffDay(DateTime.Now, p.Data1),
                    Data = EF.Functions.DateFromParts(2021, 1, 2),
                    DataValida = EF.Functions.IsDate(p.Data2),
                });

                foreach (var f in dados)
                    Console.WriteLine(f);
            }
        }

        static void FuncaoLike()
        {
            using (var db = new Data.ApplicationContext())
            {
                var script = db.Database.GenerateCreateScript();
                Console.WriteLine(script);

                var dados = db
                    .Funcoes
                    .AsNoTracking()
                    .Where(p => EF.Functions.Like(p.Descricao1, "B[ao]%"))
                    .Select(p => p.Descricao1)
                    .ToArray();

                Console.WriteLine("Resultado: ");
                foreach (var descricao in dados)
                    Console.WriteLine(descricao);
            }
        }

        static void FuncaoDataLength()
        {
            using (var db = new Data.ApplicationContext())
            {
                var resultado = db
                    .Funcoes
                    .AsNoTracking()
                    .Select(p => new
                    {
                        TotalBytesCampoData = EF.Functions.DataLength(p.Data1),
                        TotalBytes1 = EF.Functions.DataLength(p.Descricao1),
                        TotalBytes2 = EF.Functions.DataLength(p.Descricao2),
                        Total1 = p.Descricao1.Length,
                        Total2 = p.Descricao2.Length
                    })
                    .FirstOrDefault();

                Console.WriteLine("Resultado: ");
                Console.WriteLine(resultado);
            }
        }

        static void FuncaoProperty()
        {
            ApagarCriarBD();

            using (var db = new Data.ApplicationContext())
            {
                var resultado = db
                    .Funcoes
                    //.AsNoTracking()
                    .FirstOrDefault(p => EF.Property<string>(p, "PropriedadeSombra") == "Teste");

                var propriedadeSombra = db
                    .Entry(resultado)
                    .Property<string>("PropriedadeSombra")
                    .CurrentValue;

                Console.WriteLine("Resultado:");
                Console.WriteLine(propriedadeSombra);
            }
        }

        static void FuncaoCollate()
        {
            using (var db = new Data.ApplicationContext())
            {
                var consulta1 = db
                    .Funcoes
                    .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CS_AS") == "tela");

                var consulta2 = db
                    .Funcoes
                    .FirstOrDefault(p => EF.Functions.Collate(p.Descricao1, "SQL_Latin1_General_CP1_CI_AS") == "tela");

                Console.WriteLine($"Consulta 1: {consulta1?.Descricao1 ?? "Pesquisa Com CaseSensitive, verifique sua consulta"}");
                Console.WriteLine($"Consulta 2: {consulta2?.Descricao1 ?? "Não há registros correspondente"}");
            }
        }

        #endregion

        #region [+ Interceptação]

        static void TesteInterceptacao()
        {
            using (var db = new Data.ApplicationContext())
            {
                var consulta = db
                    .Funcoes
                    .TagWith("Use NOLOCK")
                    .FirstOrDefault();

                Console.WriteLine($"Consulta: {consulta.Descricao1}");
            }
        }

        static void TesteInterceptacaoSaveChanges()
        {
            using (var db = new Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Funcoes.Add(new Funcao
                {
                    Descricao1 = "Teste"
                });

                db.SaveChanges();
            }
        }

        #endregion

        #region [+ Transações]

        static void CadastrarLivro()
        {
            using (var db = new Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Introdução ao Entity Framework Core",
                        Autor = "Rafael",
                        CadastradoEm = DateTime.Now.AddDays(-1)
                    });
                db.SaveChanges();
            }
        }

        static void ComportamentoPadrao()
        {
            CadastrarLivro();

            using (var db = new Data.ApplicationContext())
            {
                var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                livro.Autor = "Rafael Almeida";

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Rafael Almeida"
                    });

                db.SaveChanges();
            }
        }

        static void GerenciandoTransacaoManualmente()
        {
            CadastrarLivro();

            /*
                Ao utilizar o using (fazendo o Dispose do objeto, destruindo a instancia do DbContext),
                o próprio EF verifica se há transações abertas e faz um rollback.
             */
            using (var db = new Data.ApplicationContext())
            {
                var transacao = db.Database.BeginTransaction();

                var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                livro.Autor = "Rafael Almeida";
                db.SaveChanges();

                Console.ReadKey();

                db.Livros.Add(
                    new Livro
                    {
                        Titulo = "Dominando o Entity Framework Core",
                        Autor = "Rafael Almeida"
                    });

                db.SaveChanges();

                transacao.Commit(); //Sem o commit, automaticamente são descartadas as alterações.
            }
        }

        static void ReverterTransacao()
        {
            CadastrarLivro();

            using (var db = new Data.ApplicationContext())
            {
                var transacao = db.Database.BeginTransaction();

                try
                {
                    var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                    livro.Autor = "Rafael Almeida";
                    db.SaveChanges();

                    Console.ReadKey();

                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "Dominando o Entity Framework Core",
                            Autor = "Rafael Almeida".PadLeft(16, '*')
                        });

                    db.SaveChanges();

                    transacao.Commit();
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                }
            }
        }

        static void SalvarPontoTransacao()
        {
            CadastrarLivro();

            using (var db = new Data.ApplicationContext())
            {
                var transacao = db.Database.BeginTransaction();

                try
                {
                    var livro = db.Livros.FirstOrDefault(p => p.Id == 1);
                    livro.Autor = "Rafael Almeida";
                    db.SaveChanges();

                    transacao.CreateSavepoint("desfazer_apenas_insercao");

                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "ASP.NET Core Enterprise Applications",
                            Autor = "Eduardo Pires"
                        });
                    db.SaveChanges();

                    db.Livros.Add(
                        new Livro
                        {
                            Titulo = "Dominando o Entity Framework Core",
                            Autor = "Rafael Almeida".PadLeft(16, '*')
                        });
                    db.SaveChanges();

                    transacao.Commit();
                }
                catch (DbUpdateException ex)
                {
                    transacao.RollbackToSavepoint("desfazer_apenas_insercao");

                    if (ex.Entries.Count(p => p.State == EntityState.Added) == ex.Entries.Count)
                    {
                        transacao.Commit();
                    }
                }
            }
        }

        #endregion

        #region [+ UDFs]

        static void FuncaoLEFT()
        {
            CadastrarLivro();

            using var db = new Data.ApplicationContext();

            var resultado = db.Livros.Select(p => MinhasFuncoes.Left(p.Titulo, 10));
            foreach (var parteTitulo in resultado)
            {
                Console.WriteLine(parteTitulo);
            }
        }

        static void FuncaoDefinidaPeloUsuario()
        {
            CadastrarLivro();

            using var db = new Data.ApplicationContext();

            db.Database.ExecuteSqlRaw(@"
                CREATE FUNCTION ConverterParaLetrasMaiusculas(@dados VARCHAR(100))
                RETURNS VARCHAR(100)
                BEGIN
                    RETURN UPPER(@dados)
                END");

            var resultado = db.Livros.Select(p => UDFs.MinhasFuncoes.LetrasMaiusculas(p.Titulo));
            foreach (var tituloMaiusculo in resultado)
            {
                Console.WriteLine(tituloMaiusculo);
            }
        }

        static void DateDIFF()
        {
            CadastrarLivro();

            using var db = new Data.ApplicationContext();

            //var resultado = db
            //    .Livros
            //    .Select(p => EF.Functions.DateDiffDay(p.CadastradoEm, DateTime.Now));

            // Documentação do DATEDIFF ↓
            // https://docs.microsoft.com/pt-br/sql/t-sql/functions/datediff-transact-sql?view=sql-server-ver15

            var resultado = db
                .Livros
                .Select(p => MinhasFuncoes.DateDiff("DAY", p.CadastradoEm, DateTime.Now));

            foreach (var diff in resultado)
            {
                Console.WriteLine(diff);
            }
        }

        #endregion

        #region [+ Performance]

        static void Setup()
        {
            using var db = new Data.ApplicationContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.Add(new Departamento
            {
                Descricao = "Departamento Teste",
                Ativo = true,
                Funcionarios = Enumerable.Range(1, 100).Select(p => new Funcionario
                {
                    CPF = p.ToString().PadLeft(11, '0'),
                    Nome = $"Funcionario {p}",
                    RG = p.ToString(),
                }).ToList()
            });

            db.SaveChanges();
        }

        static void ConsultaRastreada()
        {
            using var db = new Data.ApplicationContext();

            var funcionarios = db.Funcionarios.Include(p => p.Departamento).ToList();
        }

        static void ConsultaNaoRastreada()
        {
            using var db = new Data.ApplicationContext();

            var funcionarios = db.Funcionarios.AsNoTracking().Include(p => p.Departamento).ToList();
        }

        static void ConsultaComResoluçãoDeIdentidade()
        {
            // Método indicado para consultas somente leitura

            using var db = new Data.ApplicationContext();

            var funcionarios = db
                .Funcionarios
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Departamento)
                .ToList();
        }

        static void ConsultaProjetadaERastreada()
        {
            using var db = new Data.ApplicationContext();

            var departamentos = db.Departamentos
                .Include(p => p.Funcionarios)
                .Select(p => new
                {
                    Departamento = p,
                    TotalFuncionarios = p.Funcionarios.Count()
                })
                .ToList();
            departamentos[0].Departamento.Descricao = "Departamento Teste Atualizado";
            db.SaveChanges();
        }

        static void Inserir_200_Departamentos_Com_1MB()
        {
            using var db = new Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var random = new Random();

            db.Departamentos.AddRange(Enumerable.Range(1, 200).Select(p =>
                new Departamento
                {
                    Descricao = "Departamento Teste",
                    Image = GetByte()
                }));

            db.SaveChanges();

            byte[] GetByte()
            {
                var buffer = new byte[1024 * 1024];
                random.NextBytes(buffer);

                return buffer;
            }
        }

        static void ConsultaProjetadaComImagem()
        {
            using var db = new Data.ApplicationContext();

            // var departamentos = db.Departamentos.ToArray(); // Query com consumo de 300 MB+
            var departamentos = db.Departamentos.Select(p => p.Descricao).ToArray();

            var memoria = (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024 + " MB");

            Console.WriteLine(memoria);
        }

        #endregion

        #region [+ Migrações]

        // Nuget Packages necessários:
        // Microsoft.EntityFrameworkCore.Design
        // Microsoft.EntityFrameworkCore.Tools

        #endregion

        #region [+ Diagnostics]

        static void TesteDiagnostics()
        {
            DiagnosticListener.AllListeners.Subscribe(new MyInterceptorListener());

            using var db = new Data.ApplicationContext();
            db.Database.EnsureCreated();

            _ = db.Departamentos.Where(p => p.Id > 0).ToArray();
        }

        #endregion
    }
}