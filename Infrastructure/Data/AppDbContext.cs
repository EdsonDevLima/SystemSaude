using Microsoft.EntityFrameworkCore;

namespace SystemSaude.Infrastructure.Data{
    public class AppDbContext:DbContext
{
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options) {}
    
}
}