namespace Library.Application.Configuration;

public sealed class LibraryConstraintsOptions
{
    public const string SectionName = "LibraryConstraints";

    public int MaxBooksAllowed { get; init; } = 1000;
}

