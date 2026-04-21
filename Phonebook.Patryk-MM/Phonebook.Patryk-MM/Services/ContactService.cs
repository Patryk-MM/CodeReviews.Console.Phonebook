using Microsoft.Identity.Client;
using Phonebook.Patryk_MM.Models;
using Phonebook.Patryk_MM.Repositories;
using Spectre.Console;

namespace Phonebook.Patryk_MM.Services;
public class ContactService : IContactService {
    private readonly ContactRepository _repository;

    public ContactService(ContactRepository repository) {
        _repository = repository;
    }

    public async Task ViewContacts() {

        List<Contact> contacts = await _repository.GetAllAsync();

        var prompt = new SelectionPrompt<Contact>()
            .Title("Choose contact")
            .PageSize(10)
            .MoreChoicesText("Move up or down")
            .EnableSearch()
            .SearchPlaceholderText("Type to search")
            .WrapAround()
            .HighlightStyle(new Style(Color.Gold1, decoration: Decoration.RapidBlink))
            .UseConverter(c => $"{c.ToString()}")
            .AddChoices(contacts);

        var selected = AnsiConsole.Prompt(prompt);
    }
}