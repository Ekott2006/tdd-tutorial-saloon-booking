using Contract;
using Domain.DTOs;
using Domain.Model;
using Domain.Repository;

namespace Domain.Services;

public class EmployeeHelper(ShiftRepository shiftRepository, AppointmentRepository appointmentRepository)
{
    public async Task<List<ShiftsAndAppointment>> GetShiftsAndAppointment(Guid? id, Period period)
    {
        Task<Dictionary<Guid, IEnumerable<Shift>>> shiftTask = shiftRepository.GetShifts(period, id);
        Task<Dictionary<Guid, IEnumerable<Appointment>>> appointmentTask = appointmentRepository.GetAppointments(period, id);

        // Wait for both tasks to complete
        await Task.WhenAll(shiftTask, appointmentTask);

        // Get results from tasks
        Dictionary<Guid, IEnumerable<Shift>> employeeShifts = shiftTask.Result;
        Dictionary<Guid, IEnumerable<Appointment>> employeeAppointments = appointmentTask.Result;
        
        IEnumerable<Guid> shiftsId = employeeShifts.Select(x => x.Key)
            .Union(employeeAppointments.Select(x => x.Key));
        return shiftsId.Select(s => new ShiftsAndAppointment(
            employeeShifts.GetValueOrDefault(s, []),
            employeeAppointments.GetValueOrDefault(s, []))
        ).ToList();
    }

    public async Task<List<Guid>> VerifyAppointment(Period newAppPeriod, Guid? id = null)
    {
        // Get all shifts that 
        Dictionary<Guid, IEnumerable<Shift>>? res =await shiftRepository.GetShifts(new Period(DateTime.MinValue, DateTime.MaxValue));
        Task<Dictionary<Guid, IEnumerable<Shift>>> shiftTask = shiftRepository.BookingValidShifts(newAppPeriod, id);
        Task<List<Guid>?> appointmentTask = appointmentRepository.GetInvalidEmployeeIds(newAppPeriod, id);

        // Wait for both tasks to complete
        await Task.WhenAll(shiftTask, appointmentTask);

        // Get results from tasks
        Dictionary<Guid, IEnumerable<Shift>> employeeShifts = shiftTask.Result;
        List<Guid>? employeeAppointments = appointmentTask.Result;

        if (employeeShifts.Count == 0 || employeeAppointments is null) return [];
        foreach (Guid invalidId in employeeAppointments)
        {
            if (employeeShifts.ContainsKey(invalidId))
            {
                employeeShifts.Remove(invalidId);
            }
        }
        return employeeShifts.Select(x => x.Key).ToList();
    }

}