using Microsoft.AspNetCore.Mvc.Filters;

namespace GymHub.Api.Core;

public class ModelBindingFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var errors = context.ModelState
            .Where(kv => kv.Key.StartsWith('$') && kv.Value is { Errors.Count: > 0 })
            .Select(kv => $"El campo '{kv.Key.TrimStart('$', '.')}' no tiene un formato válido")
            .ToList();
        if (errors.Count > 0) throw new DataValidationException(errors);

        var missingBody = context.ActionDescriptor.Parameters
            .Any(p => p.BindingInfo?.BindingSource?.Id == "Body"
                      && context.ActionArguments.TryGetValue(p.Name, out var v) && v is null);
        if (missingBody) throw new DataValidationException("El cuerpo de la petición es obligatorio");
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
