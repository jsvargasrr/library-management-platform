namespace Library.Application.Books.Dtos;

public sealed record BookUpdateRequest(
    string Title,
    int Year,
    string Genre,
    int Pages,
    Guid AuthorId);

