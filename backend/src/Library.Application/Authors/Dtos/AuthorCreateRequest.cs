namespace Library.Application.Authors.Dtos;

public sealed record AuthorCreateRequest(
    string FullName,
    DateOnly? BirthDate,
    string? City,
    string Email);

