namespace Domain.Model;

public class Appointment: Period
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }

    public Appointment()
    {
        
    }

    public Appointment(Guid employeeId,  Guid customerId,  Period period)
    {
        EmployeeId = employeeId;
        CustomerId = customerId;
        (StartDateTime, EndDateTime) = period;
    }
}