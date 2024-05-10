namespace Domain.Model;

public class HairdressingService
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsActive { get; set; }
}