using Spectre.Console;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Phonebook.Patryk_MM.Models;

[Table("Contacts")]
public class Contact : BaseEntity {

    [Required]
    [MinLength(2)]
    [MaxLength(32)]
    public string Name { get; set; }

    [Required]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "Phone number must be exactly 9 digits.")]  //Polish nine digits phone numbers without country prefix 
    public string PhoneNumber { get; set; } = "";

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = "";
    
    [Required]
    public Category Category { get; set; }

    public Contact() {
        
    }

    public Contact(string name, string phoneNumber, string email, Category category) {
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        Category = category;
    }

    public static Color GetCategoryColor(Category c) {
        return c switch {
            Category.Family => Color.Maroon,
            Category.Friends => Color.MediumOrchid,
            Category.Work => Color.SlateBlue1,
            Category.Miscellaneous => Color.White,
            _ => Color.White
        };
    }

    public override string ToString() {
        return $"{Name} | {Email} | {PhoneNumber} | {Category}";
    }
}


public enum Category {
    Family,
    Friends, 
    Work,
    Miscellaneous
}