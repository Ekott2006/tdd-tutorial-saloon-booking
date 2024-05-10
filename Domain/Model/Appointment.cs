namespace Domain.Model;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }
    public Period Period { get; set; }
}