using Phonebook.Patryk_MM.Models;

namespace Phonebook.Patryk_MM.Repositories;
public class ContactRepository : BaseRepository<Contact>, IContactRepository {

    public ContactRepository(PhonebookDbContext _dbContext) : base(_dbContext) {
        
    }
}
