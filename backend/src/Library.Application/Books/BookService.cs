using FluentValidation;
using Library.Application.Abstractions;
using Library.Application.Books.Dtos;
using Library.Application.Common;
using Library.Application.Common.Validation;
using Library.Application.Configuration;
using Library.Domain.Authors;
using Library.Domain.Books;
using Microsoft.Extensions.Options;
using AppValidationException = Library.Application.Common.ValidationException;

namespace Library.Application.Books;

public sealed class BookService : IBookService
{
    private readonly IBookRepository _books;
    private readonly IAuthorRepository _authors;
    private readonly IOptions<LibraryConstraintsOptions> _constraints;
    private readonly IValidator<BookCreateRequest> _createValidator;
    private readonly IValidator<BookUpdateRequest> _updateValidator;

    public BookService(
        IBookRepository books,
        IAuthorRepository authors,
        IOptions<LibraryConstraintsOptions> constraints,
        IValidator<BookCreateRequest> createValidator,
        IValidator<BookUpdateRequest> updateValidator)
    {
        _books = books;
        _authors = authors;
        _constraints = constraints;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<BookResponse> CreateAsync(BookCreateRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new AppValidationException("La solicitud es inválida.", validation.ToErrorMessages());

        // Regla del enunciado: no se puede registrar un libro si el autor no existe.
        var authorExists = await _authors.ExistsAsync(request.AuthorId, ct);
        if (!authorExists)
            throw new AuthorNotRegisteredException();

        // Regla del enunciado: límite máximo de libros configurable (no “magic number”).
        var currentBooks = await _books.CountAsync(ct);
        if (currentBooks >= _constraints.Value.MaxBooksAllowed)
            throw new MaxBooksReachedException();

        var book = new Book(
            id: Guid.NewGuid(),
            title: request.Title.Trim(),
            year: request.Year,
            genre: request.Genre.Trim(),
            pages: request.Pages,
            authorId: request.AuthorId);

        await _books.AddAsync(book, ct);
        await _books.SaveChangesAsync(ct);

        var author = await _authors.GetByIdAsync(request.AuthorId, ct);
        var authorName = author?.FullName ?? string.Empty;

        return new BookResponse(
            Id: book.Id,
            Title: book.Title,
            Year: book.Year,
            Genre: book.Genre,
            Pages: book.Pages,
            AuthorId: book.AuthorId,
            AuthorName: authorName);
    }

    public async Task<List<BookResponse>> ListAsync(CancellationToken ct)
    {
        var books = await _books.ListAsync(ct);
        var authors = await _authors.ListAsync(ct);

        // Evita N+1 en lectura: resolvemos nombres de autores con un lookup en memoria.
        var authorsById = authors.ToDictionary(a => a.Id, a => a.FullName);

        return books
            .Select(b => new BookResponse(
                Id: b.Id,
                Title: b.Title,
                Year: b.Year,
                Genre: b.Genre,
                Pages: b.Pages,
                AuthorId: b.AuthorId,
                AuthorName: authorsById.GetValueOrDefault(b.AuthorId, string.Empty)))
            .OrderBy(b => b.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<BookResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var book = await _books.GetByIdAsync(id, ct);
        if (book is null)
            throw new NotFoundException("Libro no encontrado.");

        var author = await _authors.GetByIdAsync(book.AuthorId, ct);

        return new BookResponse(
            Id: book.Id,
            Title: book.Title,
            Year: book.Year,
            Genre: book.Genre,
            Pages: book.Pages,
            AuthorId: book.AuthorId,
            AuthorName: author?.FullName ?? string.Empty);
    }

    public async Task<BookResponse> UpdateAsync(Guid id, BookUpdateRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new AppValidationException("La solicitud es inválida.", validation.ToErrorMessages());

        var book = await _books.GetByIdAsync(id, ct);
        if (book is null)
            throw new NotFoundException("Libro no encontrado.");

        var authorExists = await _authors.ExistsAsync(request.AuthorId, ct);
        if (!authorExists)
            throw new AuthorNotRegisteredException();

        book.Update(
            title: request.Title.Trim(),
            year: request.Year,
            genre: request.Genre.Trim(),
            pages: request.Pages,
            authorId: request.AuthorId);

        await _books.SaveChangesAsync(ct);

        var author = await _authors.GetByIdAsync(book.AuthorId, ct);

        return new BookResponse(
            Id: book.Id,
            Title: book.Title,
            Year: book.Year,
            Genre: book.Genre,
            Pages: book.Pages,
            AuthorId: book.AuthorId,
            AuthorName: author?.FullName ?? string.Empty);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var book = await _books.GetByIdAsync(id, ct);
        if (book is null)
            throw new NotFoundException("Libro no encontrado.");

        await _books.DeleteAsync(book, ct);
        await _books.SaveChangesAsync(ct);
    }
}

