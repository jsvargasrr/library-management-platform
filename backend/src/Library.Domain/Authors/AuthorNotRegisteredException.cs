using Library.Domain.Common;

namespace Library.Domain.Authors;

public sealed class AuthorNotRegisteredException : DomainException
{
    public const string RequiredMessage = "El autor no está registrado";

    public AuthorNotRegisteredException() : base(RequiredMessage)
    {
    }
}

