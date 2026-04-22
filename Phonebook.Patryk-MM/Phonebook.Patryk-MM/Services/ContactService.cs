using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Phonebook.Patryk_MM.Models;
using Phonebook.Patryk_MM.Repositories;
using Spectre.Console;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Phonebook.Patryk_MM.Services;
public class ContactService : IContactService {
    private readonly ContactRepository _repository;
    private List<Contact> _contacts;

    public ContactService(ContactRepository repository) {
        _repository = repository;
    }

    public async Task ViewContacts() {
        Utility.ClearConsole();

        _contacts = await _repository.GetAllAsync();

        SelectionPrompt<Contact> prompt = new SelectionPrompt<Contact>()
            .Title("Choose contact")
            .PageSize(10)
            .MoreChoicesText("Move up or down")
            .EnableSearch()
            .SearchPlaceholderText("Type to search")
            .WrapAround()
            .HighlightStyle(new Style(Color.LightGreen, decoration: Decoration.RapidBlink))
            .UseConverter(c => $"{c.ToString()}")
            .AddChoices(_contacts);

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

        Contact c = new Contact(name, phoneNumber, email, category);

        if (ContactValidator.ValidateContact(c)) {
            try {
                await _repository.AddAsync(c);
                AnsiConsole.MarkupLine("[green]Contact successfully created.[/]");
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627)) {
                AnsiConsole.MarkupLine("[red]A phone number already exists in the database.[/]\n");
            }
            catch (Exception ex) {
                AnsiConsole.MarkupLine($"[red]{ex.InnerException?.Message}[/]\n");
            }
        }
        else {
            AnsiConsole.MarkupLine("[yellow]There were validation errors. Please create a contact once again.[/]");
        }
    }

    public async Task EditContact(Contact c) {
        TextPrompt<string> prompt = new TextPrompt<string>("Contact's name:")
            .DefaultValue(c.Name)
            .Validate(input => input.Trim().Length > 0 && input.Trim().Length < 32, "[red]Name cannot be empty or longer than 32 characters.[/]");

        c.Name = AnsiConsole.Prompt(prompt).Trim();

        prompt = new TextPrompt<string>("Contact's phone number:")
           .DefaultValue(c.PhoneNumber)
           .Validate(input => {
               var regex = new Regex(@"^\d{9}$");
               return regex.IsMatch(input.Trim()) ? ValidationResult.Success() : ValidationResult.Error("[red]Please input a valid phone number.[/]");
           });

        c.PhoneNumber = AnsiConsole.Prompt(prompt).Trim();

        prompt = new TextPrompt<string>("Contact's email:")
            .DefaultValue(c.Email)
            .Validate(input => {
                var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                return regex.IsMatch(input.Trim()) ? ValidationResult.Success() : ValidationResult.Error("[red]Please input a valid e-mail address.[/]");
            });

        c.Email = AnsiConsole.Prompt(prompt).Trim();

        SelectionPrompt<Category> categoryPrompt = new SelectionPrompt<Category>()
            .Title("Pick a category:")
            .DefaultValue(c.Category)
            .AddChoices(Enum.GetValues<Category>())
            .UseConverter(c => $"[{Contact.GetCategoryColor(c)}]{c}[/]");

        c.Category = AnsiConsole.Prompt(categoryPrompt);

        if (ContactValidator.ValidateContact(c)) {
            try {
                await _repository.EditAsync(c);
                AnsiConsole.MarkupLine("[green]Contact successfully edited.[/]");
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627)) {
                AnsiConsole.MarkupLine("[red]A phone number already exists in the database.[/]\n");
            }
            catch (Exception ex) {
                AnsiConsole.MarkupLine($"[red]{ex.InnerException?.Message}[/]\n");
            }
        }
        else {
            AnsiConsole.MarkupLine("[yellow]There were validation errors. Please create a contact once again.[/]");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public async Task DeleteContact(Contact c) {
        if (AnsiConsole.Confirm($"\n[yellow]Are you sure you want to delete contact [bold]{c.Name}[/]?[/]")) {
            try {
                await _repository.DeleteAsync(c);
                AnsiConsole.MarkupLine("[green]Contact successfully deleted.[/]");
            }
            catch (Exception ex) {
                // Safely falls back to ex.Message if there is no InnerException
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                AnsiConsole.MarkupLine($"[red]Error deleting contact: {errorMessage}[/]\n");
            }
        }
        else {
            AnsiConsole.MarkupLine("[grey]Deletion cancelled.[/]");
        }
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
                    await EditContact(c);
                    Utility.ClearConsole();
                    await DisplayContact(c);
                    break;
                case "Delete contact":
                    await DeleteContact(c);
                    break;
                case "Go back":
                    Utility.ClearConsole();
                    return;
            }
        }
    }

    public async Task DisplayContact(Contact c) {
        Contact contactToDisplay = await _repository.GetByIdAsync(c.Id);

        Panel panel = new Panel($"Phone number: {contactToDisplay.PhoneNumber}\n" +
            $"Email address: {contactToDisplay.Email}\nCategory: [{Contact.GetCategoryColor(contactToDisplay.Category).ToString()}]{contactToDisplay.Category}[/]")
            .Header($"[{Contact.GetCategoryColor(contactToDisplay.Category).ToString()}]{c.Name}[/]")
            .DoubleBorder()
            .BorderColor(Contact.GetCategoryColor(contactToDisplay.Category));

        AnsiConsole.Write(panel);
    }
}

