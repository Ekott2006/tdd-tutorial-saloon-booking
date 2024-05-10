using Domain.Model;
using Domain.Repository;
using Domain.Test.Unit.ModelFaker;

namespace Domain.Test.Unit.RepositoryTests;

public class ShiftRepositoryTests : DatabaseHelperTestsBase
{
    [Fact]
    public async Task GetShifts_ShouldReturnEmptyDictionary_WhenPeriodIsInvalid()
    {
        // Arrange
        Period period = new(DateTime.UtcNow, DateTime.UtcNow.AddHours(-1));
        ShiftRepository repository = new(Context); 

        // Act
        Dictionary<Guid, IEnumerable<Shift>> result = await repository.GetShifts(period);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetShifts_ShouldReturnAllShifts_WhenNoEmployeeIdProvided()
    {
        // Arrange
        DateTime dateTime = DateTime.Now;
        Period period = new() { StartDateTime = dateTime.AddDays(-1), EndDateTime = dateTime.AddDays(1) };
        List<Employee> employee = new EmployeeFaker().Generate(3);
        Shift shift1 = new(employee.First().Id, new Period(dateTime.AddHours(3), dateTime.AddHours(5)));
        Shift shift2 = new(employee[1].Id, new Period(dateTime, dateTime.AddHours(1)));
        await Context.Employees.AddRangeAsync(employee);
        await Context.Shifts.AddRangeAsync([shift1, shift2]);
        await Context.SaveChangesAsync();
        
        Dictionary<Guid, List<Shift>> expectedShifts = new()
        {
            { shift1.EmployeeId, [shift1] },
            { shift2.EmployeeId, [shift2] }
        };
        
        ShiftRepository repository = new(Context);

        // Act
        Dictionary<Guid, IEnumerable<Shift>> result = await repository.GetShifts(period);

        // Assert
        Assert.Equal(expectedShifts.Keys.Order(), result.Keys.Order());
    }

    [Fact]
    public async Task GetShifts_ShouldReturnShiftsForSpecificEmployee_WhenEmployeeIdProvided()
    {
        // Arrange
        DateTime dateTime = DateTime.Now;
        Period period = new() { StartDateTime = dateTime.AddDays(-1), EndDateTime = dateTime };
        List<Employee> employee = new EmployeeFaker().Generate(3);
        
        Shift shift1 = new(employee.First().Id, new Period(dateTime.AddHours(-2), dateTime.AddHours(-1)));
        Shift shift2 = new(employee[1].Id, new Period(dateTime, dateTime.AddHours(1)));
        await Context.Employees.AddRangeAsync(employee);
        await Context.Shifts.AddRangeAsync([shift1, shift2]);
        await Context.SaveChangesAsync();

        ShiftRepository repository = new(Context);

        // Act
        Dictionary<Guid, IEnumerable<Shift>> result = await repository.GetShifts(period, employee.First().Id);

        // Assert
        Assert.Single(result);
        Assert.Contains(result.Values.First(), s => s.EmployeeId == employee.First().Id);
    }

    [Fact]
    public async Task GetShifts_ShouldNotReturnShiftsOutsidePeriod()
    {
        // Arrange
        DateTime dateTime = DateTime.Now;
        Period period = new() { StartDateTime = dateTime, EndDateTime = dateTime.AddHours(1) };
        List<Employee> employee = new EmployeeFaker().Generate(3);

        Shift shift1 = new(employee.First().Id, dateTime.AddDays(-1),  dateTime);
        Shift shift2 = new(employee.First().Id, dateTime.AddHours(2), dateTime.AddHours(3));
        await Context.Employees.AddRangeAsync(employee);
        await Context.Shifts.AddRangeAsync([shift1, shift2]);
        await Context.SaveChangesAsync();

        ShiftRepository repository = new(Context);

        // Act
        Dictionary<Guid, IEnumerable<Shift>> result = await repository.GetShifts(period);

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task BookingValidShifts_ShouldReturnEmptyDictionary_WhenPeriodIsInvalid()
    {
        // Arrange
        Period period = new(DateTime.UtcNow, DateTime.UtcNow.AddHours(-1));
        ShiftRepository repository = new(Context); 

        // Act
        Dictionary<Guid, IEnumerable<Shift>> result = await repository.BookingValidShifts(period);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task BookingValidShifts_ShouldReturnAllShifts_WhenNoEmployeeIdProvided()
    {
        // Arrange
        DateTime dateTime = DateTime.Now;
        Period period = new() { StartDateTime = dateTime.AddDays(-1), EndDateTime = dateTime.AddDays(1) };
        List<Employee> employee = new EmployeeFaker().Generate(3);
        Shift shift1 = new(employee.First().Id, new Period(dateTime.AddHours(3), dateTime.AddHours(5)));
        Shift shift2 = new(employee[1].Id, new Period(dateTime, dateTime.AddHours(1)));
        await Context.Employees.AddRangeAsync(employee);
        await Context.Shifts.AddRangeAsync([shift1, shift2]);
        await Context.SaveChangesAsync();
        
        Dictionary<Guid, List<Shift>> expectedShifts = new()
        {
            { shift1.EmployeeId, [shift1] },
            { shift2.EmployeeId, [shift2] }
        };
        
        ShiftRepository repository = new(Context);

        // Act
        Dictionary<Guid, IEnumerable<Shift>> result = await repository.BookingValidShifts(period);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task BookingValidShifts_ShouldReturnShiftsForSpecificEmployee_WhenEmployeeIdProvided()
    {
        // Arrange
        DateTime dateTime = DateTime.Now;
        Period period = new() { StartDateTime = dateTime.AddDays(-1), EndDateTime = dateTime };
        List<Employee> employee = new EmployeeFaker().Generate(3);
        
        Shift shift1 = new(employee.First().Id, new Period(dateTime.AddHours(-2), dateTime.AddHours(-1)));
        Shift shift2 = new(employee[1].Id, new Period(dateTime, dateTime.AddHours(1)));
        await Context.Employees.AddRangeAsync(employee);
        await Context.Shifts.AddRangeAsync([shift1, shift2]);
        await Context.SaveChangesAsync();

        ShiftRepository repository = new(Context);

        // Act
        Dictionary<Guid, IEnumerable<Shift>> result = await repository.BookingValidShifts(period, employee.First().Id);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task BookingValidShifts_ShouldNotReturnShiftsOutsidePeriod()
    {
        // Arrange
        DateTime dateTime = DateTime.Now;
        Period period = new() { StartDateTime = dateTime, EndDateTime = dateTime.AddHours(1) };
        List<Employee> employee = new EmployeeFaker().Generate(3);

        Shift shift1 = new(employee.First().Id, dateTime.AddDays(-1),  dateTime);
        Shift shift2 = new(employee.First().Id, dateTime.AddHours(2), dateTime.AddHours(3));
        await Context.Employees.AddRangeAsync(employee);
        await Context.Shifts.AddRangeAsync([shift1, shift2]);
        await Context.SaveChangesAsync();

        ShiftRepository repository = new(Context);

        // Act
        Dictionary<Guid, IEnumerable<Shift>> result = await repository.BookingValidShifts(period);

        // Assert
        Assert.Empty(result);
    }
}