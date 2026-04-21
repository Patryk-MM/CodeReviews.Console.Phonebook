using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Phonebook.Patryk_MM.Models;

[Table("Contacts")]
public class Contact : BaseEntity {

    [Required]
    public string Name { get; set; }
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }
    [Required]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "Phone number must be exactly 9 digits.")]  //Polish nine digits phone numbers without country prefix 
    public string PhoneNumber { get; set; }
    [Required]
    public Category Category { get; set; }

    public override string ToString() {
        return $"{Name} | {Email} | {PhoneNumber}";
    }
}


public enum Category {
    Family,
    Friends, 
    Work
}