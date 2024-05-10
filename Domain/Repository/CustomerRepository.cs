using Domain.Data;
using Domain.Model;

namespace Domain.Repository;

public class CustomerRepository(DataContext context)
{
    public async Task<Guid> Create(string firstName, string lastName)
    {
        Customer customer = new(firstName, lastName);
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();
        return customer.Id;
    }
}