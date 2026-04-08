namespace Library.Application.Abstractions;

public interface IDateTimeProvider
{
    DateOnly TodayDateOnly { get; }
}

