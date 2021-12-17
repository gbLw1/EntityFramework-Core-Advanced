using EFCore.Multitenant.Data;
using EFCore.Multitenant.Domain;
using EFCore.Multitenant.Extensions;
using EFCore.Multitenant.Middlewares;
using EFCore.Multitenant.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Estratégia 1 - Identificador na tabela

#warning Sensitive content: sql connection string
//const string connectionString = "Server=#;Database=#;User Id=#;Password=#;";
/*builder.Services.AddDbContext<ApplicationContext>(o => o
            .UseSqlServer(connectionString)
            .LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging());*/

// Estratégia 3 - Banco de dados
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ApplicationContext>(provider =>
{
    var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

    var httpContext = provider.GetService<IHttpContextAccessor>()?.HttpContext;
    var tenantId = httpContext?.GetTenantId();

    var connectionString = builder.Configuration.GetConnectionString(tenantId);

        optionsBuilder.UseSqlServer(connectionString)
        .LogTo(Console.WriteLine)
        .EnableSensitiveDataLogging();

    return new ApplicationContext(optionsBuilder.Options);
});


/* População do banco manualmente
void Databaseinit(IApplicationBuilder app)
{
    using var db = app.ApplicationServices
        .CreateScope()
        .ServiceProvider
        .GetRequiredService<ApplicationContext>();

    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    for (int i = 1; i <= 5; i++)
    {
        db.People.Add(new Person { Name = $"Person {i}" });
        db.Products.Add(new Product { Description = $"Product {i}" });
    }

    db.SaveChanges();
}*/

builder.Services.AddScoped<TenantData>();

var app = builder.Build();

//Databaseinit(app);

//app.UseMiddleware<TenantMiddleware>();

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

