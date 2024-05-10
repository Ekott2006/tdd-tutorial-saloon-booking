using Domain.Data;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class AppointmentRepository(DataContext context)
{
    public async Task<Guid> Create(Guid employeeId, Guid customerId, Period period)
    {
        Appointment appointment = new(employeeId, customerId, period);
        await context.Appointments.AddAsync(appointment);
        await context.SaveChangesAsync();
        return appointment.Id;
    }
    
    public async Task<Dictionary<Guid, IEnumerable<Appointment>>> GetAppointments(Period period,
        Guid? employeeId = null)
    {
        IQueryable<Appointment>? query = GetAppointmentHelper(period, employeeId);
        if (query is null) return [];
        return await query.Where(sh =>
                period.StartDateTime <= sh.StartDateTime && period.EndDateTime >= sh.EndDateTime)
            .GroupBy(x => x.EmployeeId)
            .ToDictionaryAsync(x => x.Key, x => x as IEnumerable<Appointment>);
    }

    public async Task<List<Guid>?> GetInvalidEmployeeIds(Period bookingPeriod, Guid? employeeId = null)
    {
        IQueryable<Appointment>? query = GetAppointmentHelper(bookingPeriod, employeeId);
        if (query is null) return null;
        return await query.Where(app =>
            bookingPeriod.EndDateTime.AddMinutes(5) > app.StartDateTime &&
            bookingPeriod.StartDateTime < app.EndDateTime.AddMinutes(5)).Select(x => x.EmployeeId).Distinct().ToListAsync();
    }
    
    public async Task<Dictionary<Guid, IEnumerable<Appointment>>> GetAppointmentsForCreate(Period newAppPeriod,
        Guid? employeeId = null)
    {
        IQueryable<Appointment>? query = GetAppointmentHelper(newAppPeriod, employeeId);
        if (query is null) return [];
        return await query.Where(app =>
                newAppPeriod.EndDateTime.AddMinutes(5) <= app.StartDateTime ||
                newAppPeriod.StartDateTime >= app.EndDateTime.AddMinutes(5)).GroupBy(x => x.EmployeeId)
            .ToDictionaryAsync(x => x.Key, x => x as IEnumerable<Appointment>);
    }

    private IQueryable<Appointment>? GetAppointmentHelper(Period period, Guid? employeeId = null)
    {
        if (period.EndDateTime <= period.StartDateTime) return null;
        IQueryable<Appointment> query = context.Appointments.AsNoTracking();
        if (employeeId.HasValue) query = query.Where(x => x.EmployeeId == employeeId);
        return query;
    }
}