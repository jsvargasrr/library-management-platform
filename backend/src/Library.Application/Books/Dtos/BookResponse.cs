namespace Library.Application.Books.Dtos;

public sealed record BookResponse(
    Guid Id,
    string Title,
    int Year,
    string Genre,
    int Pages,
    Guid AuthorId,
    string AuthorName);

