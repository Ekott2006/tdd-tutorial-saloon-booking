using Contract;
using Domain.Model;
using Domain.Repository;

namespace Domain;

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
        Task<Dictionary<Guid, IEnumerable<Shift>>> shiftTask = shiftRepository.GetShifts(newAppPeriod, id);
        Task<Dictionary<Guid, IEnumerable<Appointment>>> appointmentTask = appointmentRepository.GetAppointmentsForCreate(newAppPeriod, id);

        // Wait for both tasks to complete
        await Task.WhenAll(shiftTask, appointmentTask);

        // Get results from tasks
        Dictionary<Guid, IEnumerable<Shift>> employeeShifts = shiftTask.Result;
        Dictionary<Guid, IEnumerable<Appointment>> employeeAppointments = appointmentTask.Result;

        IEnumerable<Guid> shiftsId = employeeShifts.Select(x => x.Key)
            .Union(employeeAppointments.Select(x => x.Key));

        List<Guid> result = [];
        foreach (Guid guid in shiftsId)
        {
            if (employeeShifts.GetValueOrDefault(guid) is not null &&
                employeeAppointments.GetValueOrDefault(guid) is null) result.Add(guid);
        }

        return result;
    }

}