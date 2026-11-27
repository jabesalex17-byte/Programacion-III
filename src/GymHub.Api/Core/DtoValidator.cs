using System.ComponentModel.DataAnnotations;

namespace GymHub.Api.Core;

public static class DtoValidator
{
    public static void ValidateOrThrow(object dto)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);
        if (results.Count > 0)
            throw new DataValidationException(results.Select(r => r.ErrorMessage ?? "Dato inválido"));
    }
}
