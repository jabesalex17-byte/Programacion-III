using GymHub.Api.Entities;

namespace GymHub.Api.DTOs;

public class ReadMemberDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int Age { get; set; }
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }
    public bool Active { get; set; }

    public static ReadMemberDto From(Member m) => new()
    {
        Id = m.Id, FirstName = m.FirstName, LastName = m.LastName, Age = m.Age,
        Email = m.Email, PhoneNumber = m.PhoneNumber, Active = m.Active,
    };
}
