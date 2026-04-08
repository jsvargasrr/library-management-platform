using Library.Application.Authors.Dtos;

namespace Library.Application.Authors;

public interface IAuthorService
{
    Task<AuthorResponse> CreateAsync(AuthorCreateRequest request, CancellationToken ct);
    Task<List<AuthorResponse>> ListAsync(CancellationToken ct);
    Task<AuthorResponse> GetAsync(Guid id, CancellationToken ct);
    Task<AuthorResponse> UpdateAsync(Guid id, AuthorUpdateRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

