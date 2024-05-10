using Domain.Model;
using Domain.Repository;
using Domain.Test.Unit.ModelFaker;

namespace Domain.Test.Unit.RepositoryTests;

public class SalonServiceRepositoryTests : DatabaseHelperTestsBase
{
    [Fact]
    public async Task Get_WithoutSeeding_EmptyList()
    {
        // Arrange
        SalonServiceRepository repository = new(Context);

        // Act
        IEnumerable<SalonService> result = await repository.Get();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Get_ActiveAndInactiveSeeding_OnlyActiveServices()
    {
        // Arrange
        SalonServiceRepository repository = new(Context);
        const int activeServices = 2;
        List<SalonService> services =
            [..new SalonServiceFaker(true).Generate(activeServices), ..new SalonServiceFaker(false).Generate(10)];
        await Context.SalonServices.AddRangeAsync(services);
        await Context.SaveChangesAsync();

        // Act
        IEnumerable<SalonService> result = await repository.Get();

        // Assert
        Assert.Equal(activeServices, result.Count());
    }
}