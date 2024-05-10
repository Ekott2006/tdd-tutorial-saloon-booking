using Bogus;
using Contract;
using Domain.DTOs;
using Domain.Model;
using Domain.Repository;
using Domain.Services;
using Domain.Test.Unit.ModelFaker;

namespace Domain.Test.Unit;

public class AppointmentHelperTests : DatabaseHelperTestsBase
{
    [Theory]
    [InlineData(0, 2)]
    [InlineData(4, 6)]
    public async Task VerifyAppointment_TimeDoesntMatch_Empty(int start, int end)
    {
        Employee employee = new EmployeeFaker();
        DateTime dateTime = DateTime.Today.AddHours(9);
        await Context.Employees.AddAsync(employee);
        await Context.Shifts.AddAsync(new Shift(employee.Id, new Period(dateTime.AddHours(1), dateTime.AddHours(5))));
        await Context.SaveChangesAsync();

        EmployeeHelper employeeHelper = new(new ShiftRepository(Context), new AppointmentRepository(Context));
        List<Guid> result = await employeeHelper.VerifyAppointment(new Period(dateTime.AddHours(start), dateTime.AddHours(end)));
        
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(3, 5)]
    public async Task VerifyAppointment_ShiftWithAppointment_Empty(int start, int end)
    {
        Employee employee = new EmployeeFaker();
        Customer customer = new CustomerFaker();
        DateTime dateTime = DateTime.Today.AddHours(9);
        await Context.Employees.AddAsync(employee);
        await Context.Shifts.AddAsync(new Shift(employee.Id, new Period(dateTime, dateTime.AddHours(5))));
        await Context.Customers.AddAsync(customer);
        await Context.Appointments.AddAsync(new Appointment(employee.Id, customer.Id,
            new Period(dateTime.AddHours(1), dateTime.AddHours(3))));
        await Context.SaveChangesAsync();
        
        EmployeeHelper employeeHelper = new(new ShiftRepository(Context), new AppointmentRepository(Context));
        List<Guid> result = await employeeHelper.VerifyAppointment(new Period(dateTime.AddHours(start), dateTime.AddHours(end)));
        
        Assert.Empty(result);
    }
    
    [Theory]
    [InlineData(0, 2)]
    [InlineData(3, 5)]
    public async Task VerifyAppointment_ShiftWithNoConflictingAppointment_List(int start, int end)
    {
        Employee employee = new EmployeeFaker();
        Customer customer = new CustomerFaker();
        DateTime dateTime = DateTime.Today.AddHours(9);
        await Context.Employees.AddAsync(employee);
        await Context.Shifts.AddAsync(new Shift(employee.Id, new Period(dateTime, dateTime.AddHours(5))));
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();
        
        EmployeeHelper employeeHelper = new(new ShiftRepository(Context), new AppointmentRepository(Context));
        List<Guid> result = await employeeHelper.VerifyAppointment(new Period(dateTime.AddHours(start).AddMinutes(5), dateTime.AddHours(end)));
        
        Assert.NotEmpty(result);
    }
        
    [Fact]
    public async Task GetShiftsAndAppointment_NoSeeding_Empty()
    {
        // Arrange
        EmployeeHelper employeeHelper = new(new ShiftRepository(Context), new AppointmentRepository(Context));
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        Guid employeeId = await GenerateEmployee();

        // Act
        List<ShiftsAndAppointment> result = await employeeHelper.GetShiftsAndAppointment(employeeId, period);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WrongId_Null()
    {
        // Arrange

        EmployeeHelper employeeHelper = new( new ShiftRepository(Context), new AppointmentRepository(Context));
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        await GenerateEmployee();

        // Act
        List<ShiftsAndAppointment> result = await employeeHelper.GetShiftsAndAppointment(Guid.NewGuid(), period);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WithShifts_List()
    {
        // Arrange

        EmployeeHelper employeeHelper = new( new ShiftRepository(Context), new AppointmentRepository(Context));
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        Employee employee = await GenerateEmployeeAdvanced(addAppointment: false);

        // Act
        List<ShiftsAndAppointment> result = await employeeHelper.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.NotNull(result.First());
        Assert.Equal(4, result.First().Shifts.Count());
        Assert.Empty(result.First().Appointments);
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WithAppointment_List()
    {
        // Arrange

        EmployeeHelper employeeHelper = new( new ShiftRepository(Context), new AppointmentRepository(Context));
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        Employee employee = await GenerateEmployeeAdvanced(false);

        // Act
        List<ShiftsAndAppointment> result = await employeeHelper.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.NotNull(result.First());
        Assert.Empty(result.First().Shifts);
        Assert.Equal(4, result.First().Appointments.Count());
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WithShiftsAndAppointment_List()
    {
        // Arrange

        EmployeeHelper employeeHelper = new( new ShiftRepository(Context), new AppointmentRepository(Context));
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        Employee employee = await GenerateEmployeeAdvanced();

        // Act
        List<ShiftsAndAppointment> result = await employeeHelper.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.First().Shifts.Count());
        Assert.Equal(4, result.First().Appointments.Count());
    }

    [Fact]
    public async Task GetShiftsAndAppointment_WithSpecificPeriod_FilterBasedOnThePeriod()
    {
        // Arrange

        EmployeeHelper employeeHelper = new( new ShiftRepository(Context), new AppointmentRepository(Context));
        Period period = new(DateTime.Now, DateTime.Now.AddDays(1));
        Employee employee = await GenerateEmployeeAdvanced();

        // Act
        List<ShiftsAndAppointment> result = await employeeHelper.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.NotNull(result.First());
        Assert.Equal(2, result.First().Shifts.Count());
        Assert.Equal(2, result.First().Appointments.Count());
    }

    [Fact]
    public async Task GetShiftsAndAppointment_PeriodOutsideRange_EmptyList()
    {
        // Arrange

        EmployeeHelper employeeHelper = new( new ShiftRepository(Context), new AppointmentRepository(Context));
        Period period = new(DateTime.MinValue, DateTime.Now);

        Employee employee = await GenerateEmployeeAdvanced();

        // Act
        List<ShiftsAndAppointment> result = await employeeHelper.GetShiftsAndAppointment(employee.Id, period);

        // Assert
        Assert.Empty(result);
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
    
    private async Task<Guid> GenerateEmployee(int num = 1)
    {
        List<Employee> employee = new EmployeeFaker().Generate(num);
        await Context.Employees.AddRangeAsync(employee);
        await Context.SaveChangesAsync();
        return employee.First().Id;
    }
}