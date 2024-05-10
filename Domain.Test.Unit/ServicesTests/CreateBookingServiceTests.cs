using Domain.Model;
using Domain.Repository;
using Domain.Services;
using Domain.Test.Unit.ModelFaker;
using Microsoft.EntityFrameworkCore;

namespace Domain.Test.Unit.ServicesTests;

public class CreateBookingServiceTests : DatabaseHelperTestsBase
{

    [Fact]
    public async Task Create_EmployeesShiftButAppointmentAllEmployee_False()
    {
        // Arrange
        const string firstName = "Nsikak";
        const string lastName = "Ekott";
        DateTime tomorrow = DateTime.Now.AddDays(1).AddHours(7);
        (_, int expectedResult) = await SeedDatabase(tomorrow);

        CustomerRepository customerRepository = new(Context);
        AppointmentRepository appointmentRepository = new(Context);
        EmployeeHelper employeeHelper = new(new ShiftRepository(Context), appointmentRepository);
        CreateBookingService service = new(employeeHelper, customerRepository, appointmentRepository);
        CreateBookingServiceParameter parameter = new(firstName, lastName, new Period(tomorrow, tomorrow.AddHours(1)));
        
        // Act
        bool result = await service.Create(parameter);

        // Assert
        Assert.False(result);
        Assert.Equal(expectedResult, Context.Appointments.Count());
    }

    [Fact]
    public async Task Create_EmployeesShiftAllEmployee_True()
    {
        // Arrange
        const string firstName = "Nsikak";
        const string lastName = "Ekott";
        DateTime tomorrow = DateTime.Now.AddDays(1).AddHours(7);
         await SeedDatabase(tomorrow, false);

        CustomerRepository customerRepository = new(Context);
        AppointmentRepository appointmentRepository = new(Context);
        EmployeeHelper employeeHelper = new(new ShiftRepository(Context), appointmentRepository);
        CreateBookingService service = new(employeeHelper, customerRepository, appointmentRepository);
        CreateBookingServiceParameter parameter = new(firstName, lastName, new Period(tomorrow, tomorrow.AddHours(1)));
        
        // Act
        bool result = await service.Create(parameter);

        // Assert
        Assert.True(result);
        Assert.Equal(1, await Context.Appointments.CountAsync());
    }

    [Fact]
    public async Task Create_EmployeesShiftButAppointmentOneEmployee_False()
    {
        // Arrange
        const string firstName = "Nsikak";
        const string lastName = "Ekott";
        DateTime tomorrow = DateTime.Now.AddDays(1).AddHours(7);
        (Employee employee, int expectedResult) = await SeedDatabase(tomorrow);

        CustomerRepository customerRepository = new(Context);
        AppointmentRepository appointmentRepository = new(Context);
        EmployeeHelper employeeHelper = new(new ShiftRepository(Context), appointmentRepository);
        CreateBookingService service = new(employeeHelper, customerRepository, appointmentRepository);
        CreateBookingServiceParameter parameter = new(firstName, lastName, new Period(tomorrow, tomorrow.AddHours(1)), employee.Id);
        
        // Act
        bool result = await service.Create(parameter);

        // Assert
        Assert.False(result);
        Assert.Equal(expectedResult, Context.Appointments.Count());
    }
    
    [Fact]
    public async Task Create_EmployeesShiftOneEmployee_True()
    {
        // Arrange
        const string firstName = "Nsikak";
        const string lastName = "Ekott";
        DateTime tomorrow = DateTime.Now.AddDays(1).AddHours(7);
        (Employee employee, _) = await SeedDatabase(tomorrow, false);

        CustomerRepository customerRepository = new(Context);
        AppointmentRepository appointmentRepository = new(Context);
        EmployeeHelper employeeHelper = new(new ShiftRepository(Context), appointmentRepository);
        CreateBookingService service = new(employeeHelper, customerRepository, appointmentRepository);

        // Act
        bool result = await service.Create(new CreateBookingServiceParameter(firstName, lastName,
            new Period(tomorrow, tomorrow.AddHours(1)), employee.Id));

        // Assert
        Assert.True(result);
        Assert.Single( await Context.Appointments.ToListAsync());
    }
    
    private async Task<(Employee, int)> SeedDatabase(DateTime tomorrow, bool addAppointment = true)
    {
        List<Employee> employees = new EmployeeFaker().Generate(3);
        Customer customer = new CustomerFaker();
        await Context.Customers.AddAsync(customer);
        await Context.Employees.AddRangeAsync(employees);
        await Context.Shifts.AddRangeAsync([
            new Shift(employees[0].Id, new Period(tomorrow, tomorrow.AddHours(10))),
            new Shift(employees[1].Id, new Period(tomorrow, tomorrow.AddHours(5))),
            new Shift(employees[2].Id, new Period(tomorrow, tomorrow.AddHours(15))),
            new Shift(employees[1].Id, new Period(tomorrow.AddDays(1), tomorrow.AddDays(1).AddHours(10))),
            new Shift(employees[2].Id, new Period(tomorrow.AddDays(2), tomorrow.AddDays(2).AddHours(10))),
        ]);
        if (addAppointment)
        {
            await Context.Appointments.AddRangeAsync([
                new Appointment(employees[0].Id, customer.Id,
                    new Period(tomorrow, tomorrow.AddHours(3))),
                new Appointment(employees[1].Id, customer.Id,
                    new Period(tomorrow, tomorrow.AddHours(3))),
                new Appointment(employees[2].Id, customer.Id,
                    new Period(tomorrow, tomorrow.AddHours(3))),
            ]);
        }

        await Context.SaveChangesAsync();
        return (await Context.Employees.FirstAsync(), await Context.Appointments.CountAsync());
    }

}