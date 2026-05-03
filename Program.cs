using Microsoft.EntityFrameworkCore;
using System.Reflection;
using SystemSaude.Application;
using SystemSaude.Infrastructure;
using SystemSaude.Infrastructure.Data;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("connectionDefault")));
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureRepositories();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "SystemSaude API",
        Version = "v1",
        Description = "Documentacao da API para consultas, pacientes e medicos."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "SystemSaude API v1");
    options.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

