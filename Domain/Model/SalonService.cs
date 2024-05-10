namespace Domain.Model;

public class SalonService
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public TimeSpan Duration { get; set; }
    public bool IsActive { get; set; }
}