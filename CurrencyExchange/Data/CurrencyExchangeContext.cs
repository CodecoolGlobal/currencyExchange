using CurrencyExchange.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Data
{
    public class CurrencyExchangeContext : DbContext
    {
        public CurrencyExchangeContext(DbContextOptions<CurrencyExchangeContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Statement> Statements { get; set; }
    }
}
