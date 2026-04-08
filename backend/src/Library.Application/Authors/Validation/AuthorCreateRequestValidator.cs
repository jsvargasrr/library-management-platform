using FluentValidation;
using Library.Application.Abstractions;
using Library.Application.Authors.Dtos;

namespace Library.Application.Authors.Validation;

public sealed class AuthorCreateRequestValidator : AbstractValidator<AuthorCreateRequest>
{
    public AuthorCreateRequestValidator(IDateTimeProvider clock)
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.City)
            .MaximumLength(120);

        RuleFor(x => x.BirthDate)
            .Must(date => date is null || date <= clock.TodayDateOnly)
            .WithMessage("La fecha de nacimiento no puede ser futura.");
    }
}

