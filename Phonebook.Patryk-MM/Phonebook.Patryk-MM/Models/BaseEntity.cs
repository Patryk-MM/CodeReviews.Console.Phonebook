using System.ComponentModel.DataAnnotations;

namespace Phonebook.Patryk_MM.Models;
public abstract class BaseEntity {
    [Key]
    public Guid Id { get; set; }
}
