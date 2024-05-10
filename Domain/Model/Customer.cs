namespace Domain.Model;

public class Customer()
{
    public Customer(string firstName, string lastName) : this()
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}