namespace Domain.DTOs;

public class SalonServiceResponse()
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public TimeSpan Duration { get; set; }
    public SalonServiceResponse(Guid id, string name, double price, TimeSpan timeSpan): this()
    {
        Id = id;
        Name = name;
        Price = price;
        Duration = timeSpan;
    }

}

