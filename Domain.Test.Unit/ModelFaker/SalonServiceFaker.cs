using Bogus;
using Domain.Model;

namespace Domain.Test.Unit.ModelFaker;

public class HairdressingServiceFaker: Faker<SalonService>
{
    public HairdressingServiceFaker()
    {
        RuleFor(x => x.Duration, f => f.Date.Timespan())
    }
}