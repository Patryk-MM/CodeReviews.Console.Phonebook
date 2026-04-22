using Phonebook.Patryk_MM.Models;
using Spectre.Console;
using System.ComponentModel.DataAnnotations;

namespace Phonebook.Patryk_MM;
public static class ContactValidator {
    public static bool ValidateContact(Contact c) {
        ValidationContext context = new ValidationContext(c);
        List<System.ComponentModel.DataAnnotations.ValidationResult> results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        bool isValid = Validator.TryValidateObject(c, context, results, true);

        if (!isValid) {
            foreach (System.ComponentModel.DataAnnotations.ValidationResult result in results) {
                AnsiConsole.MarkupLine($"[red]Error: {result.ErrorMessage}[/]");
            }
            return false;
        }

        return true;
    }
}
