using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Okane.Infrastructure.Responses;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Okane.Infrastructure.Services;

public class ValidationErrorResponseFactory
{
    public IActionResult Build(ModelStateDictionary modelState)
    {
        var errors = modelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            kvp => ToCamelCase(kvp.Key),
                            kvp => kvp.Value!.Errors
                                .Select(e => e.ErrorMessage)
                                .ToArray() as object
                        );

        return new UnprocessableEntityObjectResult(ResponsesFacade.UnprocessableEntity("Invalid data provided", new ReadOnlyDictionary<string, object?>(errors)));
    }

    private string ToCamelCase(string path)
    {
        string[] parts = path.Split('.');
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = JsonNamingPolicy.CamelCase.ConvertName(parts[i]);
        }
        return string.Join(".", parts);
    }
}
