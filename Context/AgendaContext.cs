using Microsoft.EntityFrameworkCore;
using Vistoria_projeto.Models;

namespace Vistoria_projeto.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ChecklistVistoria> ChecklistsVistorias { get; set; }
    }
}
