using Microsoft.EntityFrameworkCore;
using SystemSaude.Infrastructure.Data;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("connectionDefault")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
};

app.UseAuthentication();
app.UseAuthorization();


app.Run();

