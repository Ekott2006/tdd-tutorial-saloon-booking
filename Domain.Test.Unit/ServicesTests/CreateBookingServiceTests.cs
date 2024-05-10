using Domain.Repository;

namespace Domain.Test.Unit.ServicesTests;

public class CreateBookingServiceTests
{
    [Fact]
    public async Task Create_CorrectValues_OnlyDataInserted()
    {
        // Arrange
        CustomerRepository repository = new(Context);
        const string firstName = "Nsikak";
        const string lastName = "Ekott";

        // Act
        await repository.Create(firstName, lastName);

        // Assert
        Assert.Single(await Context.Customers.ToListAsync());
    }
    
    [Fact]
    public async Task Create_CorrectValues_ShowsInsertedValue()
    {
        // Arrange
        CustomerRepository repository = new(Context);
        const string firstName = "Nsikak";
        const string lastName = "Ekott";

        // Act
        Guid result = await repository.Create(firstName, lastName);

        // Assert
        Assert.Equal(firstName, (await Context.Customers.ToListAsync()).First().FirstName);
        Assert.Equal(lastName, (await Context.Customers.ToListAsync()).First().LastName);
        Assert.Equal(result, (await Context.Customers.ToListAsync()).First().Id);
        Assert.NotEqual(result, default);
    }
}