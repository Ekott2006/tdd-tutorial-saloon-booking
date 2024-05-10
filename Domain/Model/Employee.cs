namespace Domain.Model;

// Employee or Barber
public class Employee
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public List<Shift> Shifts { get; set; } = [];
    public List<Appointment> Appointments { get; set; } = [];
}