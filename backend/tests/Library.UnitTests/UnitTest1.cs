using FluentAssertions;
using FluentValidation;
using Library.Application.Abstractions;
using Library.Application.Authors;
using Library.Application.Authors.Dtos;
using Library.Application.Authors.Validation;
using Library.Application.Books;
using Library.Application.Books.Dtos;
using Library.Application.Books.Validation;
using Library.Application.Configuration;
using Library.Domain.Authors;
using Library.Domain.Books;
using Microsoft.Extensions.Options;
using Moq;

namespace Library.UnitTests;

public sealed class AuthorServiceTests
{
    [Fact]
    public async Task CreateAsync_should_create_author_when_request_is_valid()
    {
        var repo = new Mock<IAuthorRepository>(MockBehavior.Strict);
        repo.Setup(r => r.EmailExistsAsync("john@example.com", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        repo.Setup(r => r.AddAsync(It.IsAny<Library.Domain.Authors.Author>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var clock = new Mock<IDateTimeProvider>(MockBehavior.Strict);
        clock.SetupGet(x => x.TodayDateOnly).Returns(new DateOnly(2026, 1, 1));

        var createValidator = new AuthorCreateRequestValidator(clock.Object);
        var updateValidator = new AuthorUpdateRequestValidator(clock.Object);

        var sut = new AuthorService(repo.Object, createValidator, updateValidator);

        var result = await sut.CreateAsync(new AuthorCreateRequest(
            FullName: "John Doe",
            BirthDate: new DateOnly(1990, 1, 1),
            City: "Bogotá",
            Email: "john@example.com"), CancellationToken.None);

        result.Id.Should().NotBeEmpty();
        result.FullName.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");

        repo.VerifyAll();
    }
}

public sealed class BookServiceTests
{
    [Fact]
    public async Task CreateAsync_should_create_book_when_author_exists_and_below_max()
    {
        var books = new Mock<IBookRepository>(MockBehavior.Strict);
        books.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        books.Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        books.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var authors = new Mock<IAuthorRepository>(MockBehavior.Strict);
        var authorId = Guid.NewGuid();
        authors.Setup(r => r.ExistsAsync(authorId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        authors.Setup(r => r.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Library.Domain.Authors.Author(authorId, "Jane Doe", null, null, "jane@example.com"));

        var options = Options.Create(new LibraryConstraintsOptions { MaxBooksAllowed = 2 });

        IValidator<BookCreateRequest> createValidator = new BookCreateRequestValidator();
        IValidator<BookUpdateRequest> updateValidator = new BookUpdateRequestValidator();

        var sut = new BookService(books.Object, authors.Object, options, createValidator, updateValidator);

        var result = await sut.CreateAsync(new BookCreateRequest(
            Title: "Clean Architecture",
            Year: 2017,
            Genre: "Software",
            Pages: 300,
            AuthorId: authorId), CancellationToken.None);

        result.Id.Should().NotBeEmpty();
        result.AuthorId.Should().Be(authorId);
        result.AuthorName.Should().Be("Jane Doe");

        books.VerifyAll();
        authors.VerifyAll();
    }

    [Fact]
    public async Task CreateAsync_should_throw_when_author_does_not_exist_with_required_message()
    {
        var books = new Mock<IBookRepository>(MockBehavior.Strict);
        var authors = new Mock<IAuthorRepository>(MockBehavior.Strict);

        var authorId = Guid.NewGuid();
        authors.Setup(r => r.ExistsAsync(authorId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var options = Options.Create(new LibraryConstraintsOptions { MaxBooksAllowed = 10 });

        var sut = new BookService(
            books.Object,
            authors.Object,
            options,
            new BookCreateRequestValidator(),
            new BookUpdateRequestValidator());

        var act = () => sut.CreateAsync(new BookCreateRequest(
            Title: "Any",
            Year: 2020,
            Genre: "Any",
            Pages: 1,
            AuthorId: authorId), CancellationToken.None);

        await act.Should().ThrowAsync<AuthorNotRegisteredException>()
            .WithMessage(AuthorNotRegisteredException.RequiredMessage);

        authors.VerifyAll();
    }

    [Fact]
    public async Task CreateAsync_should_throw_when_max_books_reached_with_required_message()
    {
        var books = new Mock<IBookRepository>(MockBehavior.Strict);
        var authors = new Mock<IAuthorRepository>(MockBehavior.Strict);

        var authorId = Guid.NewGuid();
        authors.Setup(r => r.ExistsAsync(authorId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        books.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);

        var options = Options.Create(new LibraryConstraintsOptions { MaxBooksAllowed = 5 });

        var sut = new BookService(
            books.Object,
            authors.Object,
            options,
            new BookCreateRequestValidator(),
            new BookUpdateRequestValidator());

        var act = () => sut.CreateAsync(new BookCreateRequest(
            Title: "Any",
            Year: 2020,
            Genre: "Any",
            Pages: 1,
            AuthorId: authorId), CancellationToken.None);

        await act.Should().ThrowAsync<MaxBooksReachedException>()
            .WithMessage(MaxBooksReachedException.RequiredMessage);

        books.VerifyAll();
        authors.VerifyAll();
    }
}
