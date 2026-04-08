using Library.Application.Abstractions;
using Library.Domain.Books;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _db;

    public BookRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);

    public Task<int> CountAsync(CancellationToken ct)
        => _db.Books.CountAsync(ct);

    public async Task AddAsync(Book book, CancellationToken ct)
        => await _db.Books.AddAsync(book, ct);

    public Task DeleteAsync(Book book, CancellationToken ct)
    {
        _db.Books.Remove(book);
        return Task.CompletedTask;
    }

    public Task<List<Book>> ListAsync(CancellationToken ct)
        => _db.Books
            .AsNoTracking()
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}

