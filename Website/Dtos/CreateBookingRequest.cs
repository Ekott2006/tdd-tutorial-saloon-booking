using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Website.Dtos;

public class CreateBookingRequest
{
    [DisplayName("Select Employee: ")] public Guid? EmployeeId { get; set; }
    [DisplayName("Start Date: ")] [Required] public DateOnly StartDate { get; set; }
    [DisplayName("Start Time: ")] [Required] public TimeOnly StartTime { get; set; }

    [Required]
    [DisplayName("First name: ")]
    public string FirstName { get; set; }

    [Required]
    [DisplayName("Last name: ")]
    public string LastName { get; set; }
}