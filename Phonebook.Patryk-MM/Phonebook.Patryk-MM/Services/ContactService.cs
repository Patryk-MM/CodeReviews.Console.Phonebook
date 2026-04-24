using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Phonebook.Patryk_MM.Models;
using Phonebook.Patryk_MM.Repositories;
using Spectre.Console;
using System.Text.RegularExpressions;

namespace Phonebook.Patryk_MM.Services;
public class ContactService : IContactService {
    private readonly ContactRepository _repository;
    private readonly EmailService _emailService;

    public ContactService(ContactRepository repository, EmailService emailService) {
        _repository = repository;
        _emailService = emailService;
    }

    public async Task ViewContacts() {
        Utility.ClearConsole();

        var contacts = await _repository.GetAllAsync();

        if (!contacts.Any()) {
            AnsiConsole.MarkupLine("[yellow]No contacts found! Add someone first.[/]");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            return;
        }

        SelectionPrompt<Contact> prompt = new SelectionPrompt<Contact>()
            .Title($"\n[bold]Choose a contact [grey]or click ESC to go back[/]...\n\n{Utility.CenterHeader("Name",34)} | {Utility.CenterHeader("Phone number", 14)} | {Utility.CenterHeader("Email address", 32)} | {Utility.CenterHeader("Category", 14)}[/]")
            .PageSize(10)
            .MoreChoicesText("Move up or down...")
            .EnableSearch()
            .SearchPlaceholderText("Type to search...")
            .WrapAround()
            .HighlightStyle(new Style(Color.LightGreen, decoration: Decoration.RapidBlink))
            .UseConverter(c => $"{c.Name,-32} | {c.PhoneNumber,-14} | {c.Email,-32} | [{Contact.GetCategoryColor(c.Category).ToString()}]{c.Category,-14}[/]")
            .AddChoices(contacts)
            .AddCancelResult(new Contact {
                Name = "cancel"
            });

        Contact selected = AnsiConsole.Prompt(prompt);

        if (selected.Name == "cancel") return;
        
        await DisplayContact(selected);

        await ManageContact(selected);
    }

    public async Task AddContact() {
        string name, email, phoneNumber;
        Category category;

        TextPrompt<string> prompt = new TextPrompt<string>("Input contact's name [grey]or 'cancel'[/]:")
            .Validate(input => {
                if (input.ToLower() == "cancel") {
                    return ValidationResult.Success();
                }
                else if (input.Trim().Length > 0 && input.Trim().Length < 32) {
                    return ValidationResult.Success();
                }
                else {
                    return ValidationResult.Error("[red]Name cannot be empty or longer than 32 characters.[/]");
                }
            });

        name = AnsiConsole.Prompt(prompt).Trim();

        if (name.ToLower() == "cancel") return;

        prompt = new TextPrompt<string>("Input contact's phone number ([underline]XXXXXXXXX[/]) [grey]or 'cancel'[/]:")
            .Validate(input => {
                var regex = new Regex(@"^\d{9}$");

                if (input.ToLower() == "cancel") {
                    return ValidationResult.Success();
                }
                else if (regex.IsMatch(input.Trim())) {
                    return ValidationResult.Success();
                }
                else {
                    return ValidationResult.Error("[red]Please input a valid phone number.[/]");
                }
            });

        phoneNumber = AnsiConsole.Prompt(prompt).Trim();

        if (phoneNumber.ToLower() == "cancel") return;

        prompt = new TextPrompt<string>("Input contact's email ([underline]username@domain.com[/]) [grey]or 'cancel'[/]:")
            .Validate(input => {
                var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

                if (input.ToLower() == "cancel") {
                    return ValidationResult.Success();
                }
                else if (regex.IsMatch(input.Trim())) {
                    return ValidationResult.Success();
                }
                else {
                    return ValidationResult.Error("[red]Please input a valid e-mail address.[/]");
                }
            });

        email = AnsiConsole.Prompt(prompt).Trim();

        if (email.ToLower() == "cancel") return;

        SelectionPrompt<Category> categoryPrompt = new SelectionPrompt<Category>()
            .Title("Pick a category:")
            .AddChoices(Enum.GetValues<Category>())
            .UseConverter(c => $"[{Contact.GetCategoryColor(c)}]{c}[/]");

        category = AnsiConsole.Prompt(categoryPrompt);

        Contact c = new Contact(name, phoneNumber, email, category);

        await DisplayContact(c);

        if (!AnsiConsole.Confirm("Do you wish to add this contact to the phonebook?")) return;


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

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
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
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                AnsiConsole.MarkupLine($"[red]Error: {errorMessage}[/]\n");
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
                string errorMessage = ex.InnerException?.Message ?? ex.Message;
                AnsiConsole.MarkupLine($"[red]Error deleting contact: {errorMessage}[/]\n");
            }
        }
        else {
            AnsiConsole.MarkupLine("[grey]Deletion cancelled.[/]");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public async Task ManageContact(Contact c) {

        string[] options = {"Send email", "Edit contact", "Delete contact", "Go back" };

        while (true) {
            Utility.ClearConsole();
            await DisplayContact(c);

            SelectionPrompt<string> prompt = new SelectionPrompt<string>()
            .Title("Choose action")
            .AddChoices(options);

            string choice = AnsiConsole.Prompt(prompt);

            switch (choice) {
                case "Send email":
                    await _emailService.SendEmail(c);
                    break;
                case "Edit contact":
                    await EditContact(c);
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
        c = await _repository.GetByIdAsync(c.Id) ?? c; //Use the local copy if database returns null

        Panel panel = new Panel($"Phone number: {c.PhoneNumber}\n" +
            $"Email address: {c.Email}\nCategory: [{Contact.GetCategoryColor(c.Category).ToString()}]{c.Category}[/]")
            .Header($"[{Contact.GetCategoryColor(c.Category).ToString()}]{c.Name}[/]")
            .DoubleBorder()
            .BorderColor(Contact.GetCategoryColor(c.Category));

        AnsiConsole.Write(panel);
    }
}

