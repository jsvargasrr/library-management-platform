namespace Library.Application.Common;

public sealed class NotFoundException : ApplicationExceptionBase
{
    public NotFoundException(string message) : base(message)
    {
    }
}

