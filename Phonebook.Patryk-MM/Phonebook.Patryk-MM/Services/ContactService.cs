using Microsoft.Identity.Client;
using Phonebook.Patryk_MM.Models;
using Phonebook.Patryk_MM.Repositories;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace Phonebook.Patryk_MM.Services;
public class ContactService : IContactService {
    private readonly ContactRepository _repository;

    public ContactService(ContactRepository repository) {
        _repository = repository;
    }

    public async Task ViewContacts() {

        List<Contact> contacts = await _repository.GetAllAsync();

        SelectionPrompt<Contact> prompt = new SelectionPrompt<Contact>()
            .Title("Choose contact")
            .PageSize(10)
            .MoreChoicesText("Move up or down")
            .EnableSearch()
            .SearchPlaceholderText("Type to search")
            .WrapAround()
            .HighlightStyle(new Style(Color.LightGreen, decoration: Decoration.RapidBlink))
            .UseConverter(c => $"{c.ToString()}")
            .AddChoices(contacts);

        Contact selected = AnsiConsole.Prompt(prompt);

        DisplayContact(selected);

        await ManageContact(selected);
    }

    public async Task AddContact() {
        string name, email, phoneNumber;
        Category category;

        TextPrompt<string> prompt = new TextPrompt<string>("Contact's name:")
            .Validate(input => input.Trim().Length > 0 && input.Trim().Length < 32, "[red]Name cannot be empty or longer than 32 characters.[/]");

        name = AnsiConsole.Prompt(prompt).Trim();

        prompt = new TextPrompt<string>("Contact's phone number:")
           .Validate(input => {
               var regex = new Regex(@"^\d{9}$");
               return regex.IsMatch(input.Trim()) ? ValidationResult.Success() : ValidationResult.Error("[red]Please input a valid phone number.[/]");
           });

        phoneNumber = AnsiConsole.Prompt(prompt).Trim();

        prompt = new TextPrompt<string>("Contact's email:")
            .Validate(input => {
                var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                return regex.IsMatch(input.Trim()) ? ValidationResult.Success() : ValidationResult.Error("[red]Please input a valid e-mail address.[/]");
            });

        email = AnsiConsole.Prompt(prompt).Trim();

        SelectionPrompt<Category> categoryPrompt = new SelectionPrompt<Category>()
            .Title("Pick a category:")
            .AddChoices(Enum.GetValues<Category>())
            .UseConverter(c => $"[{Contact.GetCategoryColor(c)}]{c}[/]");

        category = AnsiConsole.Prompt(categoryPrompt);

        Contact contact = new Contact(name, phoneNumber, email, category);
        AnsiConsole.MarkupLine(contact.ToString());
    }

    public async Task ManageContact(Contact c) {
        string[] options = { "Edit contact", "Delete contact", "Go back" };

        while (true) {
            SelectionPrompt<string> prompt = new SelectionPrompt<string>()
            .Title("Choose action")
            .AddChoices(options);

            string choice = AnsiConsole.Prompt(prompt);

            switch (choice) {
                case "Edit contact":
                    break;
                case "Delete contact":
                    break;
                case "Go back":
                    return;
            }
        }
    }

    public void DisplayContact(Contact c) {
        Panel panel = new Panel($"Phone number: {c.PhoneNumber}\n" +
            $"Email address: {c.Email}\nCategory: [{Contact.GetCategoryColor(c.Category).ToString()}]{c.Category}[/]")
            .Header($"[{Contact.GetCategoryColor(c.Category).ToString()}]{c.Name}[/]")
            .DoubleBorder()
            .BorderColor(Contact.GetCategoryColor(c.Category));

        AnsiConsole.Write(panel);
    }
}

