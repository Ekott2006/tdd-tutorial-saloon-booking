using Domain.Model;
using Domain.Repository;
using Domain.Services;
using Domain.Test.Unit.ModelFaker;
using Domain.Test.Unit.RepositoryTests;
using DateTime = System.DateTime;

namespace Domain.Test.Unit.ServicesTests;

// Like Integration Testing
public class GetEmployeeTimeServiceTests : DatabaseHelperTestsBase
{
    [Fact]
    public async Task GetEmployeeTime_NoShift_Empty()
    {
        // Arrange
        EmployeeRepository employeeRepository = new(Context);
        GetEmployeeTimeService employeeTimeService = new(employeeRepository);

        // Act
        IEnumerable<Period> result = await employeeTimeService.GetEmployeeTime(new TimeSpan(1, 0, 0));

        // Assert
        // Assert.Equal(1932, result.Count());
        Assert.Empty(result);
    }

    // Example in the book
    [Fact]
    public async Task GetEmployeeTime_ExampleInTheBook_List()
    {
        // Arrange
        EmployeeRepository employeeRepository = new(Context);
        GetEmployeeTimeService employeeTimeService = new(employeeRepository);

        DateTime dateTime = DateTime.Today.AddDays(1).AddHours(9);
        await SeedDb([new Period(dateTime, dateTime.AddHours(2).AddMinutes(10))],
            [new Period(dateTime.AddMinutes(40), dateTime.AddHours(1).AddMinutes(35))]);

        // Act
        IEnumerable<Period> result = await employeeTimeService.GetEmployeeTime(new TimeSpan(0, 30, 0));

        // Assert
        Assert.Equal(3, result.Count());
        // TODO: Add the time periods
    }

    [Fact]
    public async Task GetEmployeeTime_ComplexExample_List()
    {
        // Arrange
        EmployeeRepository employeeRepository = new(Context);
        GetEmployeeTimeService employeeTimeService = new(employeeRepository);
        DateTime dateTime = DateTime.Today;

        // Employee 1
        await SeedEmployee1(dateTime);
        // Employee 2
        await SeedEmployee2(dateTime);
        // Employee 3
        await SeedEmployee3(dateTime);

        // Act
        IEnumerable<Period> result = await employeeTimeService.GetEmployeeTime(new TimeSpan(1, 0, 0));

        // Assert
        Assert.Equal(288, result.Count());
    }

    [Theory]
    [InlineData(1, 121)]
    [InlineData(2, 108)]
    [InlineData(3, 131)]
    public async Task GetEmployeeTime_ComplexExampleMatchesIndividualEmployee_List(int employeeNum, int expectedResult)
    {
        // Arrange
        EmployeeRepository employeeRepository = new(Context);
        GetEmployeeTimeService employeeTimeService = new(employeeRepository);
        DateTime dateTime = DateTime.Today;
        Guid id = await SeedEmployee(dateTime, employeeNum);

        // Act
        IEnumerable<Period> result = await employeeTimeService.GetEmployeeTime(new TimeSpan(1, 0, 0), id);

        // Assert
        Assert.Equal(expectedResult, result.Count());
    }

    private async Task<Guid> SeedDb(IEnumerable<Period> shifts, IEnumerable<Period> appointmentsPeriods)
    {
        Employee employee = new EmployeeFaker();
        Customer customer = new CustomerFaker();
        await Context.Shifts.AddRangeAsync(shifts.Select(x => new Shift(employee.Id, x)));
        await Context.Employees.AddAsync(employee);
        await Context.Customers.AddAsync(customer);
        await Context.Appointments.AddRangeAsync(appointmentsPeriods.Select(x =>
            new Appointment(employee.Id, customer.Id, x)));
        await Context.SaveChangesAsync();
        return employee.Id;
    }

    private async Task<Guid> SeedEmployee(DateTime dateTime, int employeeNumber) =>
        employeeNumber switch
        {
            1 => await SeedEmployee1(dateTime),
            2 => await SeedEmployee2(dateTime),
            3 => await SeedEmployee3(dateTime),
            _ => throw new Exception("Employee Number doesn't exists")
        };

    private async Task<Guid> SeedEmployee3(DateTime dateTime) =>
        await SeedDb([
                new Period(dateTime.AddDays(1).AddHours(14), dateTime.AddDays(1).AddHours(21)),
                new Period(dateTime.AddDays(2).AddHours(14), dateTime.AddDays(2).AddHours(21)),
                new Period(dateTime.AddDays(3).AddHours(14), dateTime.AddDays(3).AddHours(21)),
            ], [
                new Period(dateTime.AddDays(1).AddHours(14), dateTime.AddDays(1).AddHours(15)),
                new Period(dateTime.AddDays(2).AddHours(19), dateTime.AddDays(2).AddHours(20)),
                new Period(dateTime.AddDays(2).AddHours(20), dateTime.AddDays(2).AddHours(21)),
                new Period(dateTime.AddDays(3).AddHours(16), dateTime.AddDays(3).AddHours(17)),
                new Period(dateTime.AddDays(3).AddHours(19), dateTime.AddDays(3).AddHours(20)),
            ]
        );

    private async Task<Guid> SeedEmployee2(DateTime dateTime) =>
        await SeedDb([
                new Period(dateTime.AddDays(1).AddHours(8), dateTime.AddDays(1).AddHours(13)),
                new Period(dateTime.AddDays(2).AddHours(9), dateTime.AddDays(2).AddHours(15)),
                new Period(dateTime.AddDays(3).AddHours(9), dateTime.AddDays(3).AddHours(17)),
            ],
            [
                new Period(dateTime.AddDays(2).AddHours(12), dateTime.AddDays(2).AddHours(15)),
                new Period(dateTime.AddDays(3).AddHours(10), dateTime.AddDays(3).AddHours(12)),
                new Period(dateTime.AddDays(3).AddHours(16), dateTime.AddDays(3).AddHours(17)),
            ]
        );

    private async Task<Guid> SeedEmployee1(DateTime dateTime) =>
        await SeedDb([
                new Period(dateTime.AddDays(1).AddHours(9), dateTime.AddDays(1).AddHours(13)),
                new Period(dateTime.AddDays(2).AddHours(9), dateTime.AddDays(2).AddHours(17)),
                new Period(dateTime.AddDays(3).AddHours(9), dateTime.AddDays(3).AddHours(15)),
            ], [
                new Period(dateTime.AddDays(1).AddHours(9), dateTime.AddDays(1).AddHours(11)),
                new Period(dateTime.AddDays(2).AddHours(14), dateTime.AddDays(2).AddHours(16)),
            ]
        );
}