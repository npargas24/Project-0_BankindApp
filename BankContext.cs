using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BankingApp;

public class BankContext : DbContext
{
    #region DbSets

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<ChequeBookRequest> ChequeBookRequests { get; set; }

    #endregion

    #region Model Configuration

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.UseTphMappingStrategy();

            entity
                .HasDiscriminator<int>("UserType")
                .HasValue<Admin>(1)
                .HasValue<Customer>(2);
        });

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Account)
            .WithOne(a => a.Customer)
            .HasForeignKey<Account>(a => a.CustomerId);

        base.OnModelCreating(modelBuilder);
    }

    #endregion

    #region Database Configuration

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        IConfiguration configuration =
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        string connectionString =
            configuration.GetConnectionString("BankingDatabase")
            ?? throw new InvalidOperationException(
                "BankingDatabase connection string was not found.");

        optionsBuilder.UseSqlServer(connectionString);
    }

    #endregion
}