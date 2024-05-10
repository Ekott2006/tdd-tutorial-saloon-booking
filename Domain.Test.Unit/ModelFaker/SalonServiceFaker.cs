using Bogus;
using Domain.Model;

namespace Domain.Test.Unit.ModelFaker;

public sealed class SalonServiceFaker : Faker<SalonService>
{
    public SalonServiceFaker(bool isActive)
    {
        RuleFor(x => x.Duration, f => f.Date.Timespan());
        RuleFor(x => x.Id, _ => Guid.NewGuid());
        RuleFor(x => x.Name, f => f.Lorem.Sentence());
        RuleFor(x => x.Price, f => double.Parse(f.Commerce.Price()));
        RuleFor(x => x.IsActive, _ => isActive);
    }
}