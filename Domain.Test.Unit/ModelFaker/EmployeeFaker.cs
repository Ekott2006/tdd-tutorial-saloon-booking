using Bogus;
using Domain.Model;

namespace Domain.Test.Unit.ModelFaker;

public sealed class EmployeeFaker: Faker<Employee>
{
    public EmployeeFaker()
    {
        RuleFor(x => x.FullName, f => f.Name.FullName());
        RuleFor(x => x.Id, _ => Guid.NewGuid());
    }
}