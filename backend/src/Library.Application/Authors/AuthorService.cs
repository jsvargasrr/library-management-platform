using FluentValidation;
using Library.Application.Abstractions;
using Library.Application.Authors.Dtos;
using Library.Application.Common;
using Library.Application.Common.Validation;
using Library.Domain.Authors;
using AppValidationException = Library.Application.Common.ValidationException;

namespace Library.Application.Authors;

public sealed class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authors;
    private readonly IValidator<AuthorCreateRequest> _createValidator;
    private readonly IValidator<AuthorUpdateRequest> _updateValidator;

    public AuthorService(
        IAuthorRepository authors,
        IValidator<AuthorCreateRequest> createValidator,
        IValidator<AuthorUpdateRequest> updateValidator)
    {
        _authors = authors;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<AuthorResponse> CreateAsync(AuthorCreateRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new AppValidationException("La solicitud es inválida.", validation.ToErrorMessages());

        var emailTaken = await _authors.EmailExistsAsync(request.Email, excludingAuthorId: null, ct);
        if (emailTaken)
            throw new ConflictException("Ya existe un autor con el mismo correo electrónico.");

        var author = new Author(
            id: Guid.NewGuid(),
            fullName: request.FullName.Trim(),
            birthDate: request.BirthDate,
            city: string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            email: request.Email.Trim());

        await _authors.AddAsync(author, ct);
        await _authors.SaveChangesAsync(ct);

        return new AuthorResponse(
            Id: author.Id,
            FullName: author.FullName,
            BirthDate: author.BirthDate,
            City: author.City,
            Email: author.Email,
            BooksCount: 0);
    }

    public async Task<List<AuthorResponse>> ListAsync(CancellationToken ct)
    {
        var authors = await _authors.ListAsync(ct);
        return authors
            .Select(a => new AuthorResponse(
                Id: a.Id,
                FullName: a.FullName,
                BirthDate: a.BirthDate,
                City: a.City,
                Email: a.Email,
                BooksCount: a.Books.Count))
            .OrderBy(a => a.FullName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<AuthorResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var author = await _authors.GetByIdAsync(id, ct);
        if (author is null)
            throw new NotFoundException("Autor no encontrado.");

        return new AuthorResponse(
            Id: author.Id,
            FullName: author.FullName,
            BirthDate: author.BirthDate,
            City: author.City,
            Email: author.Email,
            BooksCount: author.Books.Count);
    }

    public async Task<AuthorResponse> UpdateAsync(Guid id, AuthorUpdateRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new AppValidationException("La solicitud es inválida.", validation.ToErrorMessages());

        var author = await _authors.GetByIdAsync(id, ct);
        if (author is null)
            throw new NotFoundException("Autor no encontrado.");

        var emailTaken = await _authors.EmailExistsAsync(request.Email, excludingAuthorId: id, ct);
        if (emailTaken)
            throw new ConflictException("Ya existe un autor con el mismo correo electrónico.");

        author.Update(
            fullName: request.FullName.Trim(),
            birthDate: request.BirthDate,
            city: string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            email: request.Email.Trim());

        await _authors.SaveChangesAsync(ct);

        return new AuthorResponse(
            Id: author.Id,
            FullName: author.FullName,
            BirthDate: author.BirthDate,
            City: author.City,
            Email: author.Email,
            BooksCount: author.Books.Count);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var author = await _authors.GetByIdAsync(id, ct);
        if (author is null)
            throw new NotFoundException("Autor no encontrado.");

        if (author.Books.Count > 0)
            throw new ConflictException("No es posible eliminar el autor porque tiene libros asociados.");

        await _authors.DeleteAsync(author, ct);
        await _authors.SaveChangesAsync(ct);
    }
}

