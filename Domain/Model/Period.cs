using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Model;

public class Period
{
    public void Deconstruct(out DateTime startDateTime, out DateTime endDateTime)
    {
        startDateTime = StartDateTime;
        endDateTime = EndDateTime;
    }

    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public TimeSpan Duration => EndDateTime - StartDateTime;

    public Period(DateTime startDateTime, DateTime endDateTime)
    {
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }

    public Period()
    {
        
    }
}