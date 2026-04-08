using FluentValidation;
using Library.Application.Books.Dtos;

namespace Library.Application.Books.Validation;

public sealed class BookUpdateRequestValidator : AbstractValidator<BookUpdateRequest>
{
    public BookUpdateRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(250);

        RuleFor(x => x.Genre)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(x => x.Pages)
            .GreaterThan(0);

        RuleFor(x => x.Year)
            .InclusiveBetween(1450, DateTime.UtcNow.Year + 1);

        RuleFor(x => x.AuthorId)
            .NotEmpty();
    }
}

