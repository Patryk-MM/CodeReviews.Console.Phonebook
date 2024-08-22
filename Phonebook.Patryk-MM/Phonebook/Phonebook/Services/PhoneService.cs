using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Phonebook.Models;
using Phonebook.Repositories;
using Spectre.Console;

namespace Phonebook.Services;
public class PhoneService {
    private readonly IBaseRepository<PhoneEntry> _repository;

    public PhoneService(IBaseRepository<PhoneEntry> phoneEntriesRepository) {
        _repository = phoneEntriesRepository;
    }

    private async Task<PhoneEntry> GetEntryInputAsync(PhoneEntry? entry = null) {
        // Create a new entry or use the existing one for edits
        entry ??= new PhoneEntry();


        entry.Name = await PromptForFieldAsync("Contact's name:", entry.Name);
        if (entry.Name == "cancel") return null;

        entry.PhoneNumber = await PromptForFieldAsync(
            "Contact's phone number:", entry.PhoneNumber, Validation.IsValidPhone);
        if (entry.PhoneNumber == "cancel") return null;

        entry.Email = await PromptForFieldAsync(
            "Contact's email address:", entry.Email, Validation.IsValidEmail);
        if (entry.Email == "cancel") return null;

        return entry;
    }

    private async Task<string> PromptForFieldAsync(
        string prompt, string currentValue, Func<string, bool>? validationFunc = null) {

        var result = AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
                .DefaultValue(currentValue)
                .Validate(input => {
                    if (string.Equals(input.ToLower(), "cancel")) return ValidationResult.Success();
                    return validationFunc == null || validationFunc(input)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Invalid input. Please try again.[/]");
                })
        );

        return string.IsNullOrWhiteSpace(result) ? currentValue : result;
    }

    public async Task ViewEntriesAsync() {
        var entries = await _repository.GetAsync();


        // Pagination settings
        int rowsPerPage = 10;
        int totalRows = entries.Count;
        int totalPages = (int)Math.Ceiling(totalRows / (double)rowsPerPage);

        int currentPage = 1;

        while (true) {
            Console.Clear();
            Menu.DisplayName();

            // Create a table
            var table = new Table();
            var align = new Align(table, HorizontalAlignment.Center);

            // Add columns with footer
            table.AddColumn("Name");
            table.AddColumn("Email");
            table.AddColumn("Phone Number");

            // Get the rows for the current page
            var rows = entries.Skip((currentPage - 1) * rowsPerPage).Take(rowsPerPage);

            // Add rows to the table
            foreach (var entry in rows) {
                table.AddRow(entry.Name, entry.Email, entry.PhoneNumber);
            }

            // Set table title
            table.Title = new TableTitle("[bold yellow]Phonebook Entries[/]");

            // Set table caption with navigation instructions
            table.Caption = new TableTitle($"[white]Page {currentPage}/{totalPages}[/]\n [grey]Use Left and Right arrow keys to navigate, Q to quit.[/]");

            // Render the table
            AnsiConsole.Write(align);

            // Handle pagination input
            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.RightArrow && currentPage < totalPages) {
                currentPage++;
            } else if (key == ConsoleKey.LeftArrow && currentPage > 1) {
                currentPage--;
            } else if (key == ConsoleKey.Q) {
                break;
            }
        }
    }
    public async Task AddEntryAsync() {
        AnsiConsole.MarkupLine("[yellow]Input [red]cancel[/] at any stage to cancel.[/]");
        var entry = await GetEntryInputAsync();
        if (entry == null) return;

        DataVisualization.PrintEntry(entry);

        if (AnsiConsole.Confirm("Do you want to add the created contact?")) {
            try {
                await _repository.AddAsync(entry);
                AnsiConsole.MarkupLine("[green]Contact added successfully.[/]");
            } catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true) {
                AnsiConsole.MarkupLine("[red]A contact with the same information already exists in the database.[/]\n");
            } catch (Exception ex) {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]\n");
            }
        } else {
            AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
        }
    }

    public async Task EditEntryAsync() {
        var entries = await _repository.GetAsync();
        var nameList = entries.Select(entry => entry.Name).ToList();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an entry: ")
                .AddChoices(nameList));

        var entry = await _repository.GetItemAsync(e => e.Name == choice);

        if (entry is null) {
            AnsiConsole.MarkupLine("[red]There is no such entry.[/]");
            return;
        }

        AnsiConsole.MarkupLine("[yellow]Leave field empty to keep the old value, input [red]cancel[/] to cancel.[/]");

        entry = await GetEntryInputAsync(entry);
        if (entry == null) return;

        DataVisualization.PrintEntry(entry);

        if (AnsiConsole.Confirm("Do you want to save these changes?")) {
            try {
                await _repository.EditAsync(entry);
                AnsiConsole.MarkupLine("[green]Contact edited successfully.[/]");
            } catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true) {
                AnsiConsole.MarkupLine("[red]A contact with the same information already exists in the database.[/]\n");
            } catch (Exception ex) {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]\n");
            }
        } else {
            AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]");
        }
    }


}
