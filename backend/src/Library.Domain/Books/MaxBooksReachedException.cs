using Library.Domain.Common;

namespace Library.Domain.Books;

public sealed class MaxBooksReachedException : DomainException
{
    // Mensaje exacto requerido por el enunciado (debe mantenerse sin cambios).
    public const string RequiredMessage = "No es posible registrar el libro, se alcanzó el máximo permitido.";

    public MaxBooksReachedException() : base(RequiredMessage)
    {
    }
}

