namespace WebApi;

public record GetEmployeeTimeRequest(Guid ServiceId, Guid? EmployeeId = null);