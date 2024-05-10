namespace Domain.Model;

public class Shift: Period
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; }

    public Shift()
    {
        
    }

    public Shift(Guid employeeId, Period period)
    {
        EmployeeId = employeeId;
        (StartDateTime, EndDateTime) = period;
    }

    public Shift(Guid employeeId, DateTime startDateTime, DateTime endDateTime) : this()
    {
        EmployeeId = employeeId;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }
}