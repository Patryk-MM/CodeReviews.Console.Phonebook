using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Phonebook.Models;

namespace Phonebook;
public class AppDbContext : DbContext {
    private readonly IConfiguration _configuration;
    public DbSet<PhoneEntry> PhoneEntries { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options) {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Phonebook"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PhoneEntry>()
            .HasData(
            new PhoneEntry {
                Name = "test",
                PhoneNumber = "111222333",
                Email = "test@test.com"
            });
    }
}

