using Phonebook.Models;
using Spectre.Console;

namespace Phonebook;
public static class DataVisualization {
    public static void PrintEntry(PhoneEntry entry) {
        var table = new Table();

        table.AddColumn("Name");
        table.AddColumn("Phone number");
        table.AddColumn("Email address");

        table.AddRow($"{entry.Name}", $"{entry.PhoneNumber}", $"{entry.Email}");

        AnsiConsole.Write( table );
    }
}

