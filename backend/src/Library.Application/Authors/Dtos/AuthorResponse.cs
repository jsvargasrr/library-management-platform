namespace Library.Application.Authors.Dtos;

public sealed record AuthorResponse(
    Guid Id,
    string FullName,
    DateOnly? BirthDate,
    string? City,
    string Email,
    int BooksCount);

