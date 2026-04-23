using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Phonebook.Patryk_MM.Models;
using Spectre.Console;

namespace Phonebook.Patryk_MM.Services;
public class EmailService : IEmailService {

    private readonly IConfiguration _config;

    public EmailService(IConfiguration config) {
        _config = config;
    }

    public async Task SendEmail(Contact c) {
        string subject, body;

        TextPrompt<string> prompt = new TextPrompt<string>("Subject: [grey]or 'cancel'[/]");
        subject = AnsiConsole.Prompt(prompt);

        if (subject.Trim().ToLower() == "cancel") return; 

        prompt = new TextPrompt<string>("Body: [grey]or 'cancel'[/]");
        body = AnsiConsole.Prompt(prompt);

        if (body.Trim().ToLower() == "cancel") return;


        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("Phonebook app", _config["EmailSettings:Username"]));
        message.To.Add(new MailboxAddress(c.Name, c.Email));
        message.Subject = subject;
        message.Body = new TextPart("plain") {
            Text = body
        };

        try {
            await AnsiConsole.Status().StartAsync("Sending email...", async ctx => {
                await Task.Delay(500);

                ctx.Status("Connecting to server...");

                using (var client = new SmtpClient()) {
                    client.ServerCertificateValidationCallback = (s, certificate, chain, errors) => true;

                    string hostname = _config["EmailSettings:Hostname"];
                    int port = int.Parse(_config["EmailSettings:Port"] ?? "1025");
                    string username = _config["EmailSettings:Username"];
                    string password = _config["EmailSettings:Password"];

                    await client.ConnectAsync(hostname, port, SecureSocketOptions.StartTls);

                    ctx.Status("Authenticating...");
                    await client.AuthenticateAsync(username, password);

                    ctx.Status("Delivering message...");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            });

            AnsiConsole.MarkupLine("[green]Email sent successfully![/]");
            AnsiConsole.MarkupLine("Press any key to continue...");
            Console.ReadKey();
        }
        catch (Exception ex) {
            AnsiConsole.MarkupLine($"[red]Failed to send email: {ex.Message}[/]");
        }
    }
}
