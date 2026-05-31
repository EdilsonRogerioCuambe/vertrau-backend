using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VertrauApi.Data;
using VertrauApi.Mappings;
using VertrauApi.Middleware;
using VertrauApi.Repositories;
using VertrauApi.Services;
using VertrauApi.Validations;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Banco de Dados
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Dependências
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<UsuarioProfile>());

// FluentValidation
builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUsuarioDtoValidator>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Formatar erros de validação
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new 
            {
                Message = "Erros de validação",
                Errors = errors
            });
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vertrau API", Version = "v1" });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Auto migration e Seeding (para desenvolvimento/teste rápido)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (context.Database.IsNpgsql())
    {
        context.Database.Migrate();
    }
    else
    {
        context.Database.EnsureCreated();
    }

    if (!context.Usuarios.Any())
    {
        context.Usuarios.AddRange(
            new VertrauApi.Models.Usuario { Nome = "João", Sobrenome = "Silva", Email = "joao.silva@teste.com", Genero = VertrauApi.Enums.Genero.Masculino },
            new VertrauApi.Models.Usuario { Nome = "Maria", Sobrenome = "Souza", Email = "maria.souza@teste.com", Genero = VertrauApi.Enums.Genero.Feminino },
            new VertrauApi.Models.Usuario { Nome = "Alex", Sobrenome = "Doe", Email = "alex.doe@teste.com", Genero = VertrauApi.Enums.Genero.Outro }
        );
        context.SaveChanges();
    }
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Necessário para testes de integração (WebApplicationFactory)
public partial class Program { }
