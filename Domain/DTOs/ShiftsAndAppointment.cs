using Domain.Model;

namespace Domain.DTOs;

public record ShiftsAndAppointment(IEnumerable<Period> Shifts, IEnumerable<Period> Appointments);