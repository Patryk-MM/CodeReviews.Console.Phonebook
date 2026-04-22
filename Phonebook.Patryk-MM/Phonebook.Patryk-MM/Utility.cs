using Spectre.Console;

namespace Phonebook.Patryk_MM;
public static class Utility {
    public static void DisplayName() {
        AnsiConsole.Write(
            new FigletText("Phonebook")
            .Centered()
            .Color(Color.Green));
    }

    public static void ClearConsole() {
        AnsiConsole.Clear();
        DisplayName();
    }
}
