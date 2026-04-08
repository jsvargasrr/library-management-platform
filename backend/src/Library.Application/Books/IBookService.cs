using Library.Application.Books.Dtos;

namespace Library.Application.Books;

public interface IBookService
{
    Task<BookResponse> CreateAsync(BookCreateRequest request, CancellationToken ct);
    Task<List<BookResponse>> ListAsync(CancellationToken ct);
    Task<BookResponse> GetAsync(Guid id, CancellationToken ct);
    Task<BookResponse> UpdateAsync(Guid id, BookUpdateRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

