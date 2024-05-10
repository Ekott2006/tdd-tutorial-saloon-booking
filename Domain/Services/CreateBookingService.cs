using Domain.Data;
using Domain.Model;
using Domain.Repository;

namespace Domain.Services;

public class CreateBookingService(
    EmployeeHelper employeeHelper,
    CustomerRepository customerRepository,
    AppointmentRepository appointmentRepository)
{
    public async Task<bool> Create(CreateBookingServiceParameter parameter)
    {
        List<Guid> result =
            await employeeHelper.VerifyAppointment(parameter.BookingPeriod, parameter.EmployeeId);
        Guid customerId = await customerRepository.Create(parameter.FirstName, parameter.LastName);
        Random random = new();
        if (result.Count == 0) return false;

        Guid employeeId = result[random.Next(result.Count - 1)];
        Guid? guid = await appointmentRepository.Create(employeeId, customerId, parameter.BookingPeriod);
        return guid is not null;
    }
}

public record CreateBookingServiceParameter(
    string FirstName,
    string LastName,
    Period BookingPeriod,
    Guid? EmployeeId = null);