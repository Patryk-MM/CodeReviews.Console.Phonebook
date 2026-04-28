# Console Phonebook

A rich, interactive command-line application for managing phonebook contacts. Built with C# and .NET 8, this application utilizes Entity Framework Core with SQL Server for data persistence, Spectre.Console for a beautiful CLI experience, and MailKit for email integration.

## Features
* **Contact Management:** Easily add, view, and manage your contacts from the terminal.
* **Beautiful CLI:** Uses `Spectre.Console` for interactive prompts, confirmation dialogs, and styled terminal output.
* **Database Integration:** Powered by Entity Framework Core and SQL Server to securely store all contact information.
* **Auto-Migrations:** The application automatically checks for and prompts to apply pending database migrations on startup.
* **Email Support:** Integrated with `MailKit` to provide email functionalities directly from the application.
* **Secure Configuration:** Uses .NET User Secrets to keep sensitive data (like database connection strings and email credentials) safely out of source control.

## Tech Stack
* **Language:** C# 12 / .NET 8.0
* **Data Access:** Entity Framework Core (SQL Server)
* **UI:** Spectre.Console
* **Email:** MailKit
* **Architecture:** Dependency Injection (Microsoft.Extensions.DependencyInjection)

## Getting Started

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* SQL Server (LocalDB, Express, or a standard instance)

### Installation and Setup

1. **Clone the repository:**
   ```bash
    git clone https://github.com/Patryk-MM/CodeReviews.Console.Phonebook.git
    cd CodeReviews.Console.Phonebook/Phonebook.Patryk-MM
    ```
3. **Configure User Secrets:**
    Because this project uses the `UserSecrets` manager for configuration, you will need to set up your local secrets for the database connection and email settings. Run the following commands in the project directory:
    ```bash
    dotnet user-secrets init
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=PhonebookDb;Trusted_Connection=True;"
    ```
    *(Note: Add any necessary MailKit email credentials to the user secrets as required by your `EmailService` implementation).*

4. **Run the application:**
   ```bash
    dotnet run
    ```
5. **Database Migrations:**
    Upon starting, the application will detect if there are any pending database migrations. It will prompt you:
    `There are pending database migrations. Do you want to apply them?`
    Select `Yes` to automatically build your SQL Server database schema.
