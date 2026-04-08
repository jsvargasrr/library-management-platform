namespace Library.Application.Authors.Dtos;

public sealed record AuthorUpdateRequest(
    string FullName,
    DateOnly? BirthDate,
    string? City,
    string Email);

