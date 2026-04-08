namespace Library.Application.Books.Dtos;

public sealed record BookCreateRequest(
    string Title,
    int Year,
    string Genre,
    int Pages,
    Guid AuthorId);

