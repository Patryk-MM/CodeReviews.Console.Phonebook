using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Phonebook.Patryk_MM;
using Phonebook.Patryk_MM.Repositories;
using Phonebook.Patryk_MM.Services;
using Spectre.Console;

IConfiguration configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

ServiceCollection services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);

services.AddDbContext<PhonebookDbContext>();

services.AddScoped<ContactRepository>();

services.AddScoped<ContactService>();
services.AddScoped<EmailService>();

services.AddScoped<MainMenu>();

var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<PhonebookDbContext>();
    var pendingMigrations = dbContext.Database.GetPendingMigrations();


    if (pendingMigrations.Any()) {
        //Utilities.ClearConsole();
        if (AnsiConsole.Confirm("There are pending database migrations. Do you want to apply them?")) {
            dbContext.Database.Migrate();
            AnsiConsole.MarkupLine("[green]Database migrations applied successfully.[/]");
        }
        else {
            Console.WriteLine("Closing the app...");
            return;
        }
        await Task.Delay(2000);
    }
}

AnsiConsole.Clear();

var mainMenu = serviceProvider.GetRequiredService<MainMenu>();

await mainMenu.RunAsync();