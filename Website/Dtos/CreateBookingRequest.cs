
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Website.Model;

public class CreateBookingRequest
{
    [DisplayName("Select Employee")] public Guid? EmployeeId { get; set; }
    [Required] public DateOnly StartDate { get; set; }
    [Required] public TimeOnly StartTime { get; set; }

    [Required]
    [DisplayName("First name: ")]
    public string FirstName { get; set; }

    [Required]
    [DisplayName("Last name: ")]
    public string LastName { get; set; }
}