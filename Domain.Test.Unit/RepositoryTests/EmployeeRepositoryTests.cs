using Domain.Model;
using Domain.Repository;
using Domain.Test.Unit.ModelFaker;

namespace Domain.Test.Unit.RepositoryTests;

public class EmployeeRepositoryTests : DatabaseHelperTestsBase
{
    [Fact]
    public async Task Get_WithoutSeeding_EmptyList()
    {
        // Arrange
        EmployeeRepository repository = new(Context);

        // Act
        IEnumerable<Employee> result = await repository.Get();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Get_Seeding_CountMatches()
    {
        // Arrange
        EmployeeRepository repository = new(Context);
        const int employeesCount = 5;
        await GenerateEmployee(employeesCount);

        // Act
        IEnumerable<Employee> result = await repository.Get();

        // Assert
        Assert.Equal(employeesCount, result.Count());
    }
    
    private async Task GenerateEmployee(int num = 1)
    {
        List<Employee>? employee = new EmployeeFaker().Generate(num);
        await Context.Employees.AddRangeAsync(employee);
        await Context.SaveChangesAsync();
    }
}