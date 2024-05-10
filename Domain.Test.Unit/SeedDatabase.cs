using Domain.Data;
using Domain.Model;
using Domain.Test.Unit.ModelFaker;
using Microsoft.EntityFrameworkCore;

namespace Domain.Test.Unit;

public class SeedDatabase(DataContext context)
{
    public async Task Seed()
    {
        await context.Database.EnsureCreatedAsync();
        if (!await context.Employees.AnyAsync())
        {
            await context.SalonServices.AddRangeAsync(new SalonServiceFaker(true).Generate(20));
            await context.SaveChangesAsync();
        }

        DateTime dateTime = DateTime.Today;

        if (!await context.Employees.AnyAsync())
        {
            // Employee 1
            await SeedEmployee1(dateTime);
            // Employee 2
            await SeedEmployee2(dateTime);
            // Employee 3
            await SeedEmployee3(dateTime);
        }
    }

    private async Task<Guid> SeedDb(IEnumerable<Period> shifts, IEnumerable<Period> appointmentsPeriods)
    {
        Employee employee = new EmployeeFaker();
        Customer customer = new CustomerFaker();
        await context.Shifts.AddRangeAsync(shifts.Select(x => new Shift(employee.Id, x)));
        await context.Employees.AddAsync(employee);
        await context.Customers.AddAsync(customer);
        await context.Appointments.AddRangeAsync(appointmentsPeriods.Select(x =>
            new Appointment(employee.Id, customer.Id, x)));
        await context.SaveChangesAsync();
        return employee.Id;
    }

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