
using Microsoft.EntityFrameworkCore;
using gerenciador_contatos.Models;

namespace gerenciador_contatos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ListContactsModel> ListContacts { get; set; }
        public DbSet<UserModel> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserModel>().HasIndex(u => u.Email).IsUnique();

        }
    }
}
