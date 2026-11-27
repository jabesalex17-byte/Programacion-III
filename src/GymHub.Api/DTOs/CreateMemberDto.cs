using System.ComponentModel.DataAnnotations;

namespace GymHub.Api.DTOs;

public class CreateMemberDto
{
    [Required(ErrorMessage = "Nombre es obligatorio")]
    [StringLength(50, ErrorMessage = "Nombre no puede pasar de 50 caracteres")]
    public string FirstName { get; set; } = "";

    [Required(ErrorMessage = "Apellido es obligatorio")]
    [StringLength(50, ErrorMessage = "Apellido no puede pasar de 50 caracteres")]
    public string LastName { get; set; } = "";

    [Range(12, 100, ErrorMessage = "Edad debe estar entre 12 y 100")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Correo debe ser un correo válido")]
    public string Email { get; set; } = "";

    [RegularExpression(@"^([0-9+\-\s()]{7,20})?$", ErrorMessage = "Teléfono solo puede tener números, espacios, + - ( ) (7 a 20 caracteres)")]
    public string? PhoneNumber { get; set; }

    public bool Active { get; set; } = true;
}
