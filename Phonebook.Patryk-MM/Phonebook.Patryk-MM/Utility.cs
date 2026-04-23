using Spectre.Console;

namespace Phonebook.Patryk_MM;
public static class Utility {
    public static void DisplayAppName() {
        AnsiConsole.Write(
            new FigletText("Phonebook")
            .Centered()
            .Color(Color.Green));
    }

    public static void ClearConsole() {
        AnsiConsole.Clear();
        DisplayAppName();
    }

    public static string CenterHeader(string text, int width) {
        if (text.Length >= width) return text;
        int leftPadding = (width - text.Length) / 2;
        return text.PadLeft(text.Length + leftPadding).PadRight(width);
    }
}
