using Library.Domain.Books;

namespace Library.Application.Abstractions;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task AddAsync(Book book, CancellationToken ct);
    Task DeleteAsync(Book book, CancellationToken ct);
    Task<List<Book>> ListAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

