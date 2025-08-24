using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using gerenciador_contatos.Models;

namespace gerenciador_contatos.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ListContactsModel> ListContacts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // IMPORTANTE para mapear as tabelas do Identity

            // Se quiser nomear tabela/colunas de Contacts:
            // builder.Entity<ContactModel>().ToTable("contacts");
        }
    }
}
