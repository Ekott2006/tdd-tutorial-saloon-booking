using Domain.Model;
using Domain.Repository;
using Domain.Test.Unit.ModelFaker;

namespace Domain.Test.Unit;

public class AppointmentHelperTests : DatabaseHelperTestsBase
{

    [Theory]
    [InlineData(0, 2)]
    [InlineData(4, 6)]
    public async Task CreateAppointment_TimeDoesntMatch_Empty(int start, int end)
    {
        Employee employee = new EmployeeFaker();
        DateTime dateTime = DateTime.Today.AddHours(9);
        await Context.Employees.AddAsync(employee);
        await Context.Shifts.AddAsync(new Shift(employee.Id, new Period(dateTime.AddHours(1), dateTime.AddHours(5))));
        await Context.SaveChangesAsync();

        EmployeeRepository repository = new(Context);

        List<Guid> result = await repository.VerifyAppointment(new Period(dateTime.AddHours(start), dateTime.AddHours(end)));
        
        Assert.Empty(result);
        Assert.Empty(Context.Appointments);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(3, 5)]
    public async Task CreateAppointment_ShiftWithAppointment_Empty(int start, int end)
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

        EmployeeRepository repository = new(Context);

        List<Guid> result = await repository.VerifyAppointment(new Period(dateTime.AddHours(start), dateTime.AddHours(end)));
        
        Assert.Empty(result);
        Assert.Single(Context.Appointments); // Appointment already created
    }
    
    [Theory]
    [InlineData(0, 2)]
    [InlineData(3, 5)]
    public async Task CreateAppointment_ShiftWithNoConflictingAppointment_List(int start, int end)
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

        EmployeeRepository repository = new(Context);

        List<Guid> result = await repository.VerifyAppointment(new Period(dateTime.AddHours(start), dateTime.AddHours(end)));
        
        Assert.Empty(result);
        Assert.Single(Context.Appointments); // Appointment already created
    }

}