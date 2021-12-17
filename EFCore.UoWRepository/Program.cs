using EFCore.UoWRepository.Data;
using EFCore.UoWRepository.Data.Repositories;
using EFCore.UoWRepository.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexão com o banco
#warning Sensitive content: sql connection string
const string connectionString = "Server=#;Database=#;User Id=#;Password=#;";
builder.Services.AddDbContext<ApplicationContext>(p => p
    .UseSqlServer(connectionString));

// Injeção de dependências
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<IUnityOfWork, UnityOfWork>();

// Método para popular o banco de dados manualmente.
void SeedDB(IApplicationBuilder app)
{
    using var db = app
        .ApplicationServices
        .CreateScope()
        .ServiceProvider
        .GetRequiredService<ApplicationContext>();

    if (db.Database.EnsureCreated())
    {
        db.Departamentos.AddRange(Enumerable.Range(1, 10)
            .Select(p => new Departamento
            {
                Descricao = $"Departamento - {p}",
                Colaboradores = Enumerable.Range(1, 10)
                .Select(c => new Colaborador
                {
                    Nome = $"Colaborador: {c}/{p}"
                }).ToList()
            }));

        db.SaveChanges();
    }
}



var app = builder.Build();

// Execução do método.
SeedDB(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
