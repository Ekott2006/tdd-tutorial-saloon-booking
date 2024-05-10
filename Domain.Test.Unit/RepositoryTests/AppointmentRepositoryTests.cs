using Domain.Model;
using Domain.Repository;
using Domain.Test.Unit.ModelFaker;
using Microsoft.EntityFrameworkCore;

namespace Domain.Test.Unit.RepositoryTests;

public class AppointmentRepositoryTests: DatabaseHelperTestsBase
{
    [Fact]
    public async Task Create_CorrectValues_ShowsInsertedValue()
    {
        // Arrange
        AppointmentRepository repository = new(Context);
        Employee employee = new EmployeeFaker();
        Customer customer = new CustomerFaker();
        Period period = new (DateTime.MinValue, DateTime.MaxValue);
        
        // Act
        await repository.Create(employee.Id, customer.Id, period);

        // Assert
        Assert.Equal(employee.Id, (await Context.Appointments.ToListAsync()).First().EmployeeId);
        Assert.Equal(customer.Id, (await Context.Appointments.ToListAsync()).First().CustomerId);
        Assert.Single(await Context.Appointments.ToListAsync());
    }
}