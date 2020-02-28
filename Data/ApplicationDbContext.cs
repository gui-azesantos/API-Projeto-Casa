using Microsoft.EntityFrameworkCore;

namespace API_Projeto_Casa.Data
{
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base (options) { }
        public DbSet<API_Projeto_Casa.Models.Local> Local { get; set; }

        public DbSet<API_Projeto_Casa.Models.Evento> Evento { get; set; }

      
    }
}