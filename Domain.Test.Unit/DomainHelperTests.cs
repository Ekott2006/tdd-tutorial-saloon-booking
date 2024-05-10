using Domain.Model;

namespace Domain.Test.Unit;

public class DomainHelperTests
{
    [Fact]
    public void GetAvailableSpots_SimpleNoAppointment_List()
    {
        // Arrange
        DateTime now = new (DateOnly.MinValue, new TimeOnly(9, 0, 0));
        Period shift = new(now, now.AddMinutes(45));
        List<Period> appointments = [];
        TimeSpan duration = new(0, 30, 0);

        // Act
        List<DateTime> result = Helper.GetAvailableSpots(shift, appointments, duration);

        // Assert
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void GetAvailableSpots_SimpleOneAppointmentAtTheEnd_Single()
    {
        // Arrange
        DateTime now = new (DateOnly.MinValue, new TimeOnly(9, 0, 0));
        Period shift = new(now, now.AddHours(2).AddMinutes(10));
        List<Period> appointments = [
            new Period(now.AddMinutes(35), now.AddHours(2).AddMinutes(10))
        ];
        TimeSpan duration = new(0, 30, 0);

        // Act
        List<DateTime> result = Helper.GetAvailableSpots(shift, appointments, duration);

        // Assert
        Assert.Single(result);
        Assert.Equal(now, result.First());
    }
    
    [Fact]
    public void GetAvailableSpots_SimpleOneAppointmentAtTheEnd_Double()
    {
        // Arrange
        DateTime now = new (DateOnly.MinValue, new TimeOnly(9, 0, 0));
        Period shift = new(now, now.AddHours(2).AddMinutes(10));
        List<Period> appointments = [
            new Period(now.AddMinutes(40), now.AddHours(2).AddMinutes(10))
        ];
        TimeSpan duration = new(0, 30, 0);

        // Act
        List<DateTime> result = Helper.GetAvailableSpots(shift, appointments, duration);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(now.AddMinutes(5), result[1]);

    }
    [Fact]
    public void GetAvailableSpots_SimpleOneAppointmentAtTheMiddle_List()
    {
        // Arrange
        DateTime now = new (DateOnly.MinValue, new TimeOnly(9, 0, 0));
        Period shift = new(now, now.AddHours(2).AddMinutes(10));
        List<Period> appointments = [
            new Period(now.AddMinutes(40), now.AddHours(1).AddMinutes(35))
        ];
        TimeSpan duration = new(0, 30, 0);

        // Act
        List<DateTime> result = Helper.GetAvailableSpots(shift, appointments, duration);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(now.AddHours(1).AddMinutes(40), result.Last());
    }
    
    [Fact]
    public void GetAvailableSpots_MultipleAppointmentAtTheMiddle_List()
    {
        // Arrange
        DateTime now = new (DateOnly.MinValue, new TimeOnly(9, 0, 0));
        Period shift = new(now, now.AddHours(2).AddMinutes(10));
        List<Period> appointments = [
            new Period(now.AddMinutes(40), now.AddMinutes(45)),
            new Period(now.AddMinutes(50), now.AddHours(1).AddMinutes(10)),
            new Period(now.AddHours(1).AddMinutes(15), now.AddHours(1).AddMinutes(35)),
        ];
        TimeSpan duration = new(0, 30, 0);

        // Act
        List<DateTime> result = Helper.GetAvailableSpots(shift, appointments, duration);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(now.AddHours(1).AddMinutes(40), result.Last());
    }
}