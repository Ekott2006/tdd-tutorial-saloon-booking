using System.Diagnostics;
using Bogus;
using Domain.Model;
using Domain.Repository;
using Domain.Test.Unit.ModelFaker;
using Microsoft.EntityFrameworkCore;

namespace Domain.Test.Unit.RepositoryTests;

public class AppointmentRepositoryTests : DatabaseHelperTestsBase
{
    [Fact]
    public async Task Create_CorrectValues_ShowsInsertedValue()
    {
        // Arrange
        AppointmentRepository repository = new(Context);
        Employee employee = new EmployeeFaker();
        Customer customer = new CustomerFaker();
        await Context.Employees.AddAsync(employee);
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();
        Period period = new(DateTime.MinValue, DateTime.MaxValue);

        // Act
        await repository.Create(employee.Id, customer.Id, period);

        // Assert
        Assert.Equal(employee.Id, (await Context.Appointments.ToListAsync()).First().EmployeeId);
        Assert.Equal(customer.Id, (await Context.Appointments.ToListAsync()).First().CustomerId);
        Assert.Single(await Context.Appointments.ToListAsync());
    }

    // ALL EMPLOYEES
    // When No Employees return false
    // When Employees but no shift return false
    // When Employees and shift but no match period return false
    // When employees and shift but appointment return false
    // When employees shift and no appointment return true and verify insertion (Ensure random)
    // ONE EMPLOYEE
    // When no/invalid employee return false
    // When employee but no shift return false -- REMOVE LATER
    // When Employees and shift but no match period return false
    // When employee and shift but no appointment return false -- REMOVE LATER

    [Fact]
    public async Task GetAppointments_ShouldReturnEmptyDictionary_WhenPeriodIsInvalid()
    {
        // Arrange
        Period period = new(DateTime.UtcNow, DateTime.UtcNow.AddHours(-1));
        AppointmentRepository repository = new(Context);

        // Act
        Dictionary<Guid, IEnumerable<Appointment>> result = await repository.GetAppointments(period);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAppointments_EmptyPeriod_ReturnsEmptyDictionary()
    {
        // Arrange
        AppointmentRepository appointmentRepository = new(Context);
        Period period = new(DateTime.Now.AddDays(1), DateTime.Now);

        // Act
        Dictionary<Guid, IEnumerable<Appointment>> appointments = await appointmentRepository.GetAppointments(period);

        // Assert
        Assert.Empty(appointments);
    }

    [Fact]
    public async Task GetAppointments_AllEmployeesWithAppointmentNoMatchingTime_ReturnsEmptyDictionary()
    {
        // Arrange
        AppointmentRepository appointmentRepository = new(Context);
        DateTime dateTime = DateTime.Now;
        Period period = new(DateTime.MinValue, DateTime.MinValue.AddMinutes(1));
        await GenerateEmployeeAdvanced(dateTime);

        // Act
        Dictionary<Guid, IEnumerable<Appointment>> appointments = await appointmentRepository.GetAppointments(period);

        // Assert
        Assert.Empty(appointments);
    }

    [Fact]
    public async Task GetAppointments_AllEmployeesWithAppointment_ReturnsListDictionary()
    {
        // Arrange
        AppointmentRepository appointmentRepository = new(Context);
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        await GenerateEmployeeAdvanced(DateTime.Today);

        // Act
        Dictionary<Guid, IEnumerable<Appointment>> appointments = await appointmentRepository.GetAppointments(period);

        // Assert
        Assert.Equal(2, appointments.Count);
        Assert.Equal(8, appointments.SelectMany(x => x.Value).Count());
    }

    [Fact]
    public async Task GetAppointments_OneEmployeesWithAppointment_ReturnsListDictionary()
    {
        // Arrange
        AppointmentRepository appointmentRepository = new(Context);
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        List<Employee> employees = await GenerateEmployeeAdvanced(DateTime.Today);

        // Act
        Dictionary<Guid, IEnumerable<Appointment>> appointments =
            await appointmentRepository.GetAppointments(period, employees.First().Id);

        // Assert
        Assert.Single(appointments);
        Assert.Equal(4, appointments.SelectMany(x => x.Value).Count());
    }

    [Fact]
    public async Task GetAppointments_AllEmployeesWithAppointment_ReturnsDictionary()
    {
        // Arrange
        AppointmentRepository appointmentRepository = new(Context);
        DateTime dateTime = DateTime.Today;
        Period period = new(dateTime, dateTime.AddDays(1));
        await GenerateEmployeeAdvanced(dateTime);

        // Act
        Dictionary<Guid, IEnumerable<Appointment>> appointments = await appointmentRepository.GetAppointments(period);

        // Assert
        Assert.Equal(2, appointments.Count);
        Assert.Equal(4, appointments.SelectMany(x => x.Value).Count());
    }

    [Fact]
    public async Task GetAppointments_OneEmployeeWithAppointment_ReturnsListDictionary()
    {
        // Arrange
        AppointmentRepository appointmentRepository = new(Context);
        DateTime dateTime = DateTime.Today;
        Period period = new(dateTime, dateTime.AddDays(1));
        List<Employee> employees = await GenerateEmployeeAdvanced(dateTime);

        // Act
        Dictionary<Guid, IEnumerable<Appointment>> appointments =
            await appointmentRepository.GetAppointments(period, employees.First().Id);


        // Assert
        Assert.Single(appointments);
        Assert.Equal(2, appointments.SelectMany(x => x.Value).Count());
    }

    [Fact]
    public async Task GetAppointments_OneEmployeeWithoutAppointment_Empty()
    {
        // Arrange
        AppointmentRepository appointmentRepository = new(Context);
        Period period = new(DateTime.MinValue, DateTime.MaxValue);
        List<Employee> employees = await GenerateEmployeeAdvanced(DateTime.Today);

        // Act
        Dictionary<Guid, IEnumerable<Appointment>> appointments =
            await appointmentRepository.GetAppointments(period, employees[2].Id);


        // Assert
        Assert.Empty(appointments);
    }


    [Fact]
    public async Task GetInvalidIds_ShouldReturnList_WhenBookingPeriodIsFiltered()
    {
        // Arrange
        AppointmentRepository repository = new(Context);
        DateTime dateTime = DateTime.Now.AddDays(1);
        Period period = new(dateTime, dateTime.AddHours(2));
        List<Employee> employees = await GenerateEmployeeAdvanced(DateTime.Now);

        Customer customer = new CustomerFaker();
        await Context.Appointments.AddAsync(
            new Appointment(employees.Last().Id, customer.Id,
                new Period(dateTime.AddHours(1), dateTime.AddHours(4))));
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();
        Guid employeeId = employees.Last().Id;

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal([employeeId], result);
    }

    [Fact]
    public async Task GetInvalidIds_ShouldReturnListInOneEmployee_WhenBookingPeriodIsFiltered()
    {
        // Arrange
        AppointmentRepository repository = new(Context);
        DateTime dateTime = DateTime.Now.AddDays(1);
        Period period = new(dateTime, dateTime.AddHours(2));
        List<Employee> employees = await GenerateEmployeeAdvanced(DateTime.Now);

        Customer customer = new CustomerFaker();
        await Context.Appointments.AddAsync(
            new Appointment(employees.Last().Id, customer.Id,
                new Period(dateTime.AddHours(1), dateTime.AddHours(4))));
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();
        Guid employeeId = employees.Last().Id;

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period, employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal([employeeId], result);
    }

    [Fact]
    public async Task GetInvalidIds_ShouldReturnEmptyInOneEmployee_WhenBookingPeriodIsFilteredAndNotTheEmployee()
    {
        // Arrange
        AppointmentRepository repository = new(Context);
        DateTime dateTime = DateTime.Now.AddDays(1);
        Period period = new(dateTime, dateTime.AddHours(2));
        List<Employee> employees = await GenerateEmployeeAdvanced(DateTime.Now);

        Customer customer = new CustomerFaker();
        await Context.Appointments.AddAsync(
            new Appointment(employees.Last().Id, customer.Id,
                new Period(dateTime.AddHours(1), dateTime.AddHours(4))));
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();
        Guid employeeId = employees.First().Id;

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period, employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetInvalidIds_ShouldReturnSingle_WhenBookingPeriodIsFiltered()
    {
        // Arrange
        AppointmentRepository repository = new(Context);
        DateTime dateTime = DateTime.Now.AddDays(1);
        Period period = new(dateTime, dateTime.AddHours(2));
        List<Employee> employees = await GenerateEmployeeAdvanced(DateTime.Now);

        Customer customer = new CustomerFaker();
        await Context.Appointments.AddAsync(
            new Appointment(employees.Last().Id, customer.Id,
                new Period(dateTime.AddHours(1), dateTime.AddHours(4))));
        await Context.Customers.AddAsync(customer);
        await Context.SaveChangesAsync();
        Guid employeeId = employees.Last().Id;

        // Act
        List<Guid>? result = await repository.GetInvalidEmployeeIds(period);

        // Assert
        Assert.NotNull(result);
        Assert.Equal([employeeId], result);
    }


    private async Task<List<Employee>> GenerateEmployeeAdvanced(DateTime now, bool addAppointment = true)
    {
        List<Period> periods =
        [
            new Period(now.AddHours(3), now.AddHours(6)),
            new Period(now.AddHours(1), now.AddHours(2)),
            new Period(now.AddDays(10).AddHours(10), now.AddDays(10).AddHours(16)),
            new Period(now.AddDays(2).AddHours(4), now.AddDays(2).AddHours(10)),
        ];
        List<Employee>? employees = new EmployeeFaker().Generate(3);
        List<Customer>? customer = new CustomerFaker().Generate(2);
        IEnumerable<Appointment> appointments =
        [
            ..periods.Select(x => new Appointment(employees.First().Id, customer.First().Id, x)),
            ..periods.Select(x => new Appointment(employees[1].Id, customer[1].Id, x))
        ];

        await Context.Employees.AddRangeAsync(employees);
        await Context.Customers.AddRangeAsync(customer);
        if (addAppointment) await Context.Appointments.AddRangeAsync(appointments);
        await Context.SaveChangesAsync();
        return employees;
    }
}