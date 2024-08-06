namespace Phonebook.Models;
public class PhoneEntry : BaseEntity {
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
