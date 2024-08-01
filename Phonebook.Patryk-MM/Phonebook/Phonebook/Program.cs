using Microsoft.Extensions.Configuration;
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName)
    .AddJsonFile("appsettings.json")
    .Build();

string connectionString = configuration["ConnectionStrings:Phonebook"];
Console.WriteLine(connectionString);