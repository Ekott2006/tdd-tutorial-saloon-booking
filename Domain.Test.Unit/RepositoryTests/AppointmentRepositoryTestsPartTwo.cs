using Domain.Model;
using Domain.Repository;
using Domain.Test.Unit.ModelFaker;

namespace Domain.Test.Unit.RepositoryTests;

public class AppointmentRepositoryTestsPartTwo: DatabaseHelperTestsBase
{
    // When Period is Invalid return null
    // No Appointment return empty
    // Some Appointment filter
    // Multiple Appointment return list of dictionary
    // Single Appointment
    // No Appointment
    
    [Fact]
    public async Task GetInvalidIds_ShouldReturnEmptyDictionary_WhenPeriodIsInvalid()
    {
        // Arrange
        Period period = new(DateTime.UtcNow, DateTime.UtcNow.AddHours(-1));
        AppointmentRepository repository = new(Context);

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period);

        // Assert
        Assert.Null(result);
    }

    
    // If the booking Period coincides with already appointment return list of offending Ids
    [Fact]
    public async Task GetInvalidIds_ShouldReturnList_WhenBookingPeriodIsForever()
    {
        // Arrange
        Period period = new(DateTime.MinValue.AddMinutes(30), DateTime.MaxValue.AddMinutes(-30));
        AppointmentRepository repository = new(Context);
        List<Employee> employees = await GenerateEmployeeAdvanced(DateTime.Now);

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new List<Guid> {employees[0].Id, employees[1].Id}.Order(), result.Order());
    }
    
    // If the booking Period doesn't coincide with already appointment return list of offending IDS
    [Fact]
    public async Task GetInvalidIds_ShouldReturnEmpty_WhenBookingPeriodIsTooShort()
    {
        // Arrange
        Period period = new(DateTime.MinValue.AddMinutes(30), DateTime.MinValue.AddMinutes(55));
        AppointmentRepository repository = new(Context);
        await GenerateEmployeeAdvanced(DateTime.Now);

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetInvalidIds_ShouldReturnListInOneEmployee_WhenBookingPeriodIsForever()
    {
        // Arrange
        Period period = new(DateTime.MinValue.AddMinutes(30), DateTime.MaxValue.AddMinutes(-30));
        AppointmentRepository repository = new(Context);
        List<Employee> employees = await GenerateEmployeeAdvanced(DateTime.Now);
        Guid employeeId = employees.First().Id;

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period, employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(employeeId, result.First());
    }

    [Fact]
    public async Task GetInvalidIds_ShouldReturnEmptyInOneEmployee_WhenBookingPeriodIsTooShort()
    {
        // Arrange
        Period period = new(DateTime.MinValue.AddMinutes(30), DateTime.MinValue.AddMinutes(55));
        AppointmentRepository repository = new(Context);
        List<Employee> employees = await GenerateEmployeeAdvanced(DateTime.Now);
        Guid employeeId = employees.First().Id;

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period, employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetInvalidIds_ShouldReturnListInOneEmployee_WhenBookingPeriodIsFilteredNoMatch()
    {
        // Arrange
        DateTime dateTime = DateTime.Today;
        Period period = new(dateTime, dateTime.AddHours(10));
        AppointmentRepository repository = new(Context);
        List<Employee> employees = await GenerateEmployeeAdvanced(dateTime);
        Guid employeeId = employees.First().Id;

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period, employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal([employeeId], result);
    }
    
    [Fact]
    public async Task GetInvalidIds_ShouldReturnEmptyInOneEmployee_WhenBookingPeriodIsFilteredMatch()
    {
        // Arrange
        DateTime dateTime = DateTime.Today;
        Period period = new(dateTime, dateTime.AddHours(10));
        AppointmentRepository repository = new(Context);
        List<Employee> employees = await GenerateEmployeeAdvanced(dateTime);
        Guid employeeId = employees.Last().Id;

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period, employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Theory]
    [InlineData(0, 4)]
    [InlineData(5, 7)]
    public async Task GetInvalidIds_ShouldReturnList_WhenBookingPeriodIsFilteredMatch(int start, int end)
    {
        // Arrange
        DateTime dateTime = DateTime.Today;
        Period period = new(dateTime.AddHours(start), dateTime.AddHours(end));
        AppointmentRepository repository = new(Context);
        List<Employee> employees = await GenerateEmployeeAdvanced(dateTime);

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period);

        // Assert
        Assert.NotNull(result);
        Assert.Equal([employees.First().Id], result);
    }
    
    private async Task<List<Employee>> GenerateEmployeeAdvanced(DateTime now)
    {
        List<Employee> employees = new EmployeeFaker().Generate(3);
        Customer customer = new CustomerFaker();
        IEnumerable<Appointment> appointments =
        [
            new Appointment(employees[0].Id, customer.Id, new Period(now.AddHours(3), now.AddHours(6))),
            new Appointment(employees[1].Id, customer.Id, new Period(now.AddDays(2).AddHours(10), now.AddDays(2).AddHours(12)))
        ];

        await Context.Employees.AddRangeAsync(employees);
        await Context.Customers.AddAsync(customer);
        await Context.Appointments.AddRangeAsync(appointments);
        await Context.SaveChangesAsync();
        return employees;
    }

}