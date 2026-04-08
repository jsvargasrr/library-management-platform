namespace Library.Domain.Books;

public sealed class Book
{
    private Book()
    {
        // EF Core
    }

    public Book(
        Guid id,
        string title,
        int year,
        string genre,
        int pages,
        Guid authorId)
    {
        Id = id;
        Title = title;
        Year = year;
        Genre = genre;
        Pages = pages;
        AuthorId = authorId;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public string Genre { get; private set; } = string.Empty;
    public int Pages { get; private set; }
    public Guid AuthorId { get; private set; }

    public void Update(string title, int year, string genre, int pages, Guid authorId)
    {
        Title = title;
        Year = year;
        Genre = genre;
        Pages = pages;
        AuthorId = authorId;
    }
}

