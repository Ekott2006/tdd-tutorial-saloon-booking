using Bogus;
using Contract;
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

    [Fact]
    public async Task GetShiftsAndAppointment_NoSeeding_Empty()
    {
        // Arrange
        EmployeeRepository repository = new(Context);
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        Guid employeeId = await GenerateEmployee();

        // Act
        List<ShiftsAndAppointment> result = await repository.GetShiftsAndAppointment(employeeId, period);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WrongId_Null()
    {
        // Arrange
        EmployeeRepository repository = new(Context);
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        await GenerateEmployee();

        // Act
        List<ShiftsAndAppointment> result = await repository.GetShiftsAndAppointment(Guid.NewGuid(), period);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WithShifts_List()
    {
        // Arrange
        EmployeeRepository repository = new(Context);
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        Employee employee = await GenerateEmployeeAdvanced(addAppointment: false);

        // Act
        List<ShiftsAndAppointment> result = await repository.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.NotNull(result.First());
        Assert.Equal(4, result.First().Shifts.Count());
        Assert.Empty(result.First().Appointments);
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WithAppointment_List()
    {
        // Arrange
        EmployeeRepository repository = new(Context);
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        Employee employee = await GenerateEmployeeAdvanced(false);

        // Act
        List<ShiftsAndAppointment> result = await repository.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.NotNull(result.First());
        Assert.Empty(result.First().Shifts);
        Assert.Equal(4, result.First().Appointments.Count());
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WithShiftsAndAppointment_List()
    {
        // Arrange
        EmployeeRepository repository = new(Context);
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        Employee employee = await GenerateEmployeeAdvanced();

        // Act
        List<ShiftsAndAppointment> result = await repository.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.First().Shifts.Count());
        Assert.Equal(4, result.First().Appointments.Count());
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WithSpecificPeriod_FilterBasedOnThePeriod()
    {
        // Arrange
        EmployeeRepository repository = new(Context);
        Period period = new(DateTime.Now, DateTime.Now.AddDays(1));
        Employee employee = await GenerateEmployeeAdvanced();

        // Act
        List<ShiftsAndAppointment> result = await repository.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.NotNull(result.First());
        Assert.Equal(2, result.First().Shifts.Count());
        Assert.Equal(2, result.First().Appointments.Count());
    }

    [Fact]
    public async Task GetShiftsAndAppointment_PeriodOutsideRange_EmptyList()
    {
        // Arrange
        EmployeeRepository repository = new(Context);
        Period period = new(DateTime.MinValue, DateTime.Now);

        Employee employee = await GenerateEmployeeAdvanced();

        // Act
        List<ShiftsAndAppointment> result = await repository.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.Empty(result);
    }

    private async Task<Guid> GenerateEmployee(int num = 1)
    {
        List<Employee>? employee = new EmployeeFaker().Generate(num);
        await Context.Employees.AddRangeAsync(employee);
        await Context.SaveChangesAsync();
        return employee.First().Id;
    }

    private async Task<Employee> GenerateEmployeeAdvanced(bool addShifts = true, bool addAppointment = true)
    {
        DateTime now = DateTime.Now;
        List<Period> periods =
        [
            new Period(now.AddHours(3), now.AddHours(6)),
            new Period(now.AddHours(1), now.AddHours(2)),
            new Period(now.AddDays(10).AddHours(10), now.AddDays(10).AddHours(16)),
            new Period(now.AddDays(2).AddHours(4), now.AddDays(2).AddHours(10)),
        ];
        List<Period>? periods2 = new Faker<Period>().Generate(5);
        List<Employee>? employees = new EmployeeFaker().Generate(2);
        List<Customer>? customer = new CustomerFaker().Generate(2);
        IEnumerable<Appointment> appointments =
        [
            ..periods.Select(x => new Appointment(employees.First().Id, customer.First().Id, x)),
            ..periods2.Select(x => new Appointment(employees[1].Id, customer[1].Id, x))
        ];

        if (addShifts)
        {
            await Context.Shifts.AddRangeAsync(periods.Select(x => new Shift(employees.First().Id, x)));
            await Context.Shifts.AddRangeAsync(periods2.Select(x => new Shift(employees[1].Id, x)));
        }

        await Context.Employees.AddRangeAsync(employees);
        await Context.Customers.AddRangeAsync(customer);
        if (addAppointment) await Context.Appointments.AddRangeAsync(appointments);
        await Context.SaveChangesAsync();
        await Context.SaveChangesAsync();
        return employees.First();
    }
}