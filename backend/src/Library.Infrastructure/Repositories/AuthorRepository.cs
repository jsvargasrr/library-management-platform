using Library.Application.Abstractions;
using Library.Domain.Authors;
using Library.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Repositories;

public sealed class AuthorRepository : IAuthorRepository
{
    private readonly LibraryDbContext _db;

    public AuthorRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public Task<Author?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        => _db.Authors.AnyAsync(a => a.Id == id, ct);

    public Task<bool> EmailExistsAsync(string email, Guid? excludingAuthorId, CancellationToken ct)
    {
        var query = _db.Authors.AsNoTracking().Where(a => a.Email == email);
        if (excludingAuthorId.HasValue)
            query = query.Where(a => a.Id != excludingAuthorId.Value);

        return query.AnyAsync(ct);
    }

    public async Task AddAsync(Author author, CancellationToken ct)
        => await _db.Authors.AddAsync(author, ct);

    public Task DeleteAsync(Author author, CancellationToken ct)
    {
        _db.Authors.Remove(author);
        return Task.CompletedTask;
    }

    public Task<List<Author>> ListAsync(CancellationToken ct)
        => _db.Authors
            .Include(a => a.Books)
            .AsNoTracking()
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}

