using Microsoft.EntityFrameworkCore;
using Phonebook.Models;

namespace Phonebook;
public class AppDbContext : DbContext {
    public DbSet<PhoneEntry> PhoneEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Phonebook;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PhoneEntry>()
            .HasIndex(e => e.Name)
            .IsUnique();

        modelBuilder.Entity<PhoneEntry>()
            .HasData(
            new PhoneEntry {
                Id = 1,
                Name = "test",
                PhoneNumber = "111222333",
                Email = "test@test.com"
            });
    }
}

