using Phonebook.Patryk_MM.Models;

namespace Phonebook.Patryk_MM.Services;
public interface IEmailService {
    Task SendEmail(Contact c);
}
