using System.Text.RegularExpressions;

namespace Phonebook;
public static class Validation {
    public static bool IsValidEmail(string input) {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(input, pattern);
    }
    public static bool IsValidPhone(string input) {
        string pattern = @"^(?:\+48|48)?[-\s]?\d{3}[-\s]?\d{3}[-\s]?\d{3}$";
        return Regex.IsMatch(input, pattern);
    }
}
