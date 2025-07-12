using FileUploaderDocspider.Core.Domains.Models;
using FileUploaderDocspider.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FileUploaderDocspider.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new DocumentConfiguration());
        }
    }
}
