using Domain.Model;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Contract;
using Domain.DTOs;

namespace Domain.Services;

public class GetEmployeeTimeService(EmployeeHelper employeeHelper)
{
    public async Task<List<Period>> GetEmployeeTime(TimeSpan duration, Guid? employeeId = null)
    {
        Period period = new(DateTime.Today, DateTime.Today.AddDays(7));
        List<ShiftsAndAppointment> result = await employeeHelper.GetShiftsAndAppointment(employeeId, period);
        IEnumerable<DateTime> availablePeriods = [];
        result.ForEach(x =>
        {
            availablePeriods = x.Shifts.Select(shift => Helper.GetAvailableSpots(shift, x.Appointments.ToList(), duration)).Aggregate(availablePeriods, (current, startDates) => current.Union(startDates));
        });

        return availablePeriods.Select(startDate => new Period(startDate, startDate + duration)).ToList();
    }
}