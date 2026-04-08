using Library.Domain.Authors;

namespace Library.Application.Abstractions;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, Guid? excludingAuthorId, CancellationToken ct);
    Task AddAsync(Author author, CancellationToken ct);
    Task DeleteAsync(Author author, CancellationToken ct);
    Task<List<Author>> ListAsync(CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

