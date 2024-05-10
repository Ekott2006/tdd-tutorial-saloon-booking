using Domain.Model;

namespace Contract;

public record ShiftsAndAppointment(IEnumerable<Period> Shifts, IEnumerable<Period> Appointments);