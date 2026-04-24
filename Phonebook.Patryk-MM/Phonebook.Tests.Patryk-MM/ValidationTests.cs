using Phonebook.Patryk_MM;
using Phonebook.Patryk_MM.Models;

namespace Phonebook.Tests.Patryk_MM;

public class ValidationTests {
    [Theory]
    [InlineData("Valid Name", "123456789", "test@gmail.com", Category.Miscellaneous, true)]
    [InlineData("", "123456789", "test@gmail.com", Category.Miscellaneous, false)]
    [InlineData("Valid Name", "1234567892", "test@gmail.com", Category.Miscellaneous, false)]
    [InlineData("Valid Name", "", "test@gmail.com", Category.Miscellaneous, false)]
    [InlineData("Valid Name", "123456789", "test", Category.Miscellaneous, false)]
    [InlineData("Valid Name", "123456789", "", Category.Miscellaneous, false)]
    [InlineData("n", "123456789", "test@gmail.com", Category.Miscellaneous, false)]
    [InlineData("testtesttesttesttesttesttesttesttesttesttest", "123456789", "test@gmail.com", Category.Miscellaneous, false)]
    public void ValidateContact_ShouldReturnExpectedResult(string name, string phoneNumber, string email, Category category, bool expectedResult) {
        Contact contact = new Contact(name, phoneNumber, email, category);

        bool actualResult = ContactValidator.ValidateContact(contact);

        Assert.Equal(expectedResult, actualResult);
    }
}