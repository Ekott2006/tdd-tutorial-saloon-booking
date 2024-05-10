using Domain.Data;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class ShiftRepository(DataContext context)
{
    public async Task<Dictionary<Guid, IEnumerable<Shift>>> GetShifts(Period period, Guid? employeeId = null)
    {
        if (period.EndDateTime <= period.StartDateTime) return [];
        IQueryable<Shift> query = context.Shifts.AsNoTracking();
        if (employeeId.HasValue) query = query.Where(x => x.EmployeeId == employeeId);
        return await query.Where(sh =>
                period.StartDateTime <= sh.StartDateTime && period.EndDateTime >= sh.EndDateTime)
            .GroupBy(x => x.EmployeeId)
            .ToDictionaryAsync(x => x.Key, x => x as IEnumerable<Shift>);
    }
    
    public async Task<Dictionary<Guid, IEnumerable<Shift>>> BookingValidShifts(Period period, Guid? employeeId = null)
    {
        if (period.EndDateTime <= period.StartDateTime) return [];
        IQueryable<Shift> query = context.Shifts.AsNoTracking();
        if (employeeId.HasValue) query = query.Where(x => x.EmployeeId == employeeId);
        return await query.Where(sh =>
                period.StartDateTime >= sh.StartDateTime && period.EndDateTime <= sh.EndDateTime)
            .GroupBy(x => x.EmployeeId)
            .ToDictionaryAsync(x => x.Key, x => x as IEnumerable<Shift>);
    }
}