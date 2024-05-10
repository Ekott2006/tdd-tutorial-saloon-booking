using Bogus;
using Domain.Model;

namespace Domain.Test.Unit.ModelFaker;

public sealed class CustomerFaker: Faker<Customer>
{
    public CustomerFaker()
    {
        RuleFor(x => x.FirstName, f => f.Name.FirstName());
        RuleFor(x => x.LastName, f => f.Name.LastName());
        RuleFor(x => x.Id, _ => Guid.NewGuid());
    }
}