using Phonebook.Services;
using Spectre.Console;

namespace Phonebook;
public class Menu {
    private readonly PhoneService _phoneService;

    public Menu(PhoneService phoneService) {
        _phoneService = phoneService;
    }

    public static void DisplayName() {
        AnsiConsole.Write(
            new FigletText("Phonebook")
            .Centered()
            .Color(Color.Silver));
    }

    public async Task<bool> RunAsync() {
        DisplayName();
        while (true) {
            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("[bold]Choose an option:[/]")
            .AddChoices(new[] {
                    "View entries", "Add entry", "Edit entry", "Delete entry", "[red]Exit the app[/]"
            }));

            AnsiConsole.Clear();
            DisplayName();

            switch (choice) {
                case "View entries":
                    await _phoneService.ViewEntriesAsync();
                    break;
                case "Add entry":
                    await _phoneService.AddEntryAsync();
                    break;
                case "Edit entry":
                    //await _stackManager.RunAsync();
                    break;
                case "Delete entry":
                    //await _stackManager.RunAsync();
                    break;
                case "[red]Exit the app[/]":
                    return false;
            }
        }
    }
}
