using AccountService.Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.xmin)
                .HasColumnName("xmin")
                .HasColumnType("xid")
                .IsRowVersion()
                .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<Account>()
                .HasIndex(e => e.OwnerId)
                .HasDatabaseName("IX_Account_OwnerId")
                .HasMethod("hash");

            modelBuilder.Entity<Transaction>()
                .HasIndex(e => e.TransferTime)
                .HasDatabaseName("IX_Transaction_TransferTime")
                .HasMethod("gist");

            modelBuilder.Entity<Transaction>()
                .HasIndex(t => new { t.AccountId, t.TransferTime })
                .HasDatabaseName("IX_Transaction_AccountId_TransferTime");

            modelBuilder.Entity<Account>()
                .HasMany(a => a.Transactions)
                .WithOne(t => t.Account);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CounterpartyTransactions)
                .WithOne(t => t.CounterpartyAccount);
        }
    }
}
