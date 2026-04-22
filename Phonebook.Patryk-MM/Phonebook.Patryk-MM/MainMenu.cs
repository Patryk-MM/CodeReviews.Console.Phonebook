using Phonebook.Patryk_MM.Models;
using Phonebook.Patryk_MM.Services;
using Spectre.Console;

namespace Phonebook.Patryk_MM;
public class MainMenu {
    private readonly ContactService _service;

    private readonly string[] _choices = {
        "View contacts", "Add new contact", "[red]Exit the app[/]"
    };

    public MainMenu(ContactService service) {
        _service = service;
    }

    public async Task RunAsync() {
        Utility.ClearConsole();

        while (true) {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("[bold]Choose an option:[/]")
                .AddChoices(_choices));

            Utility.ClearConsole();

            switch (choice) {
                case "View contacts":
                    await _service.ViewContacts();
                    break;
                case "Add new contact":
                    await _service.AddContact();
                    break;
                case "[red]Exit the app[/]":
                    return;
            }
        }
    }
}
