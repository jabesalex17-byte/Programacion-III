namespace GymHub.Api.Entities;

[JsonCollection("members")]
public class Member : BaseEntity
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int Age { get; set; }
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public bool Active { get; set; } = true;
}
