using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model;

[ComplexType]
public class Period
{
    public DateTime StartDateTime { get; set; }
    public TimeSpan Duration { get; set; }
}