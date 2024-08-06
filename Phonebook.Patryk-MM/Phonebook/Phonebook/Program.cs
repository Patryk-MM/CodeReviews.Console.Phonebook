using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Phonebook;
using Phonebook.Models;
using Phonebook.Repositories;
using Phonebook.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName)
    .AddJsonFile("appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);

services.AddDbContext<AppDbContext>();

services.AddScoped<IBaseRepository<PhoneEntry>, BaseRepository<PhoneEntry>>();
services.AddScoped<PhoneService>();

services.AddScoped<Menu>();