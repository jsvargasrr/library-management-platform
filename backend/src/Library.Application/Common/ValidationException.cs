namespace Library.Application.Common;

public sealed class ValidationException : ApplicationExceptionBase
{
    public ValidationException(string message, IReadOnlyCollection<string> errors) : base(message)
    {
        Errors = errors;
    }

    public IReadOnlyCollection<string> Errors { get; }
}

