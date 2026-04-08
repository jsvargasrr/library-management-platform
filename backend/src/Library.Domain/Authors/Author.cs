using Library.Domain.Books;

namespace Library.Domain.Authors;

public sealed class Author
{
    private readonly List<Book> _books = new();

    private Author()
    {
        // EF Core
    }

    public Author(
        Guid id,
        string fullName,
        DateOnly? birthDate,
        string? city,
        string email)
    {
        Id = id;
        FullName = fullName;
        BirthDate = birthDate;
        City = city;
        Email = email;
    }

    public Guid Id { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public DateOnly? BirthDate { get; private set; }
    public string? City { get; private set; }
    public string Email { get; private set; } = string.Empty;

    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    public void Update(string fullName, DateOnly? birthDate, string? city, string email)
    {
        FullName = fullName;
        BirthDate = birthDate;
        City = city;
        Email = email;
    }
}

