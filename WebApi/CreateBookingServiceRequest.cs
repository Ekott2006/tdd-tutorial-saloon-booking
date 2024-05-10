namespace WebApi;

public record CreateBookingServiceRequest(
    string FirstName,
    string LastName,
    DateTime StartDate,
    Guid ServiceId,
    Guid? EmployeeId = null);