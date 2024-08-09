using Microsoft.Extensions.DependencyInjection;
using Phonebook;
using Phonebook.Models;
using Phonebook.Repositories;
using Phonebook.Services;

var services = new ServiceCollection();

services.AddDbContext<AppDbContext>();

services.AddScoped<IBaseRepository<PhoneEntry>, BaseRepository<PhoneEntry>>();
services.AddScoped<PhoneService>();

services.AddScoped<Menu>();

var serviceProvider = services.BuildServiceProvider();

var menu = serviceProvider.GetRequiredService<Menu>();
await menu.RunAsync();