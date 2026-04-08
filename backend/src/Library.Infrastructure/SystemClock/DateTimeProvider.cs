using Library.Application.Abstractions;

namespace Library.Infrastructure.SystemClock;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateOnly TodayDateOnly => DateOnly.FromDateTime(DateTime.UtcNow);
}

