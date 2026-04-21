using Microsoft.EntityFrameworkCore;
using Phonebook.Patryk_MM.Models;

namespace Phonebook.Patryk_MM;
public class PhonebookDbContext : DbContext {

    public DbSet<Contact> Contacts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=CSharpAcademy_Phonebook;Trusted_Connection=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<Contact>()
            .HasData(
            new Contact {
                Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                Name = "Test Contact",
                Email = "test@contact.com",
                PhoneNumber = "111222333",
                Category = Category.Work
            });
    }
}
