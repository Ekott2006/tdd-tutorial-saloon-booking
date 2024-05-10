using Domain.Model;
using Domain.Repository;

namespace Domain;

public static class Helper
{
    public static List<DateTime> GetAvailableSpots(Period shift, List<Period> appointments, TimeSpan duration)
    {
        List<DateTime> dateTimes = [];
        for (DateTime current = shift.StartDateTime;
             current + duration <= shift.EndDateTime;
             current = current.AddMinutes(5))
        {
            if (appointments.Count != 0 && !appointments.TrueForAll(
                    x => current.AddMinutes(5) + duration <= x.StartDateTime ||
                         current >= x.EndDateTime.AddMinutes(5))) continue;
            dateTimes.Add(current);
        }

        return dateTimes;
    }
}