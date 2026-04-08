using FluentValidation.Results;

namespace Library.Application.Common.Validation;

public static class FluentValidationExtensions
{
    public static IReadOnlyCollection<string> ToErrorMessages(this ValidationResult result)
        => result.Errors
            .Select(e => e.ErrorMessage)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}

