using BankSysteam.Api.Controllers;
using BankSysteam.Api.models;
using Microsoft.EntityFrameworkCore;

namespace BankSysteam.Api.data
{
    public class BankContext : DbContext
    {
       public BankContext(DbContextOptions<BankContext> options) : base(options)
       {
        
       }
        //public DbSet<Account> Accounts { get; set; }
        public DbSet<Cliente> Clients { get; set; }

        public DbSet<Conta> Contas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Conta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Numero).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Balance).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Tipo).IsRequired();

                entity.HasOne(e => e.Cliente)
                      .WithMany(a => a.Conta)
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired();
                entity.Property(e => e.Email).IsRequired();
            });
        }
    }
}
