using Phonebook.Models;
using Phonebook.Repositories;
using Spectre.Console;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Phonebook.Services;
public class PhoneService {
    private readonly IBaseRepository<PhoneEntry> _repository;

    public PhoneService(IBaseRepository<PhoneEntry> phoneEntriesRepository) {
        _repository = phoneEntriesRepository;
    }

    public async Task ViewEntriesAsync() {
        var entries = await _repository.GetAllAsync();


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
            AnsiConsole.Write(table);

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
        var entry = new PhoneEntry();
    }
}
