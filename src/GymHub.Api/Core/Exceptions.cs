namespace GymHub.Api.Core;

public class NotFoundException(string message) : Exception(message);

public class DataValidationException(IEnumerable<string> errors) : Exception("Datos inválidos")
{
    public IReadOnlyList<string> Errors { get; } = errors.ToList();

    public DataValidationException(string error) : this([error]) { }
}
