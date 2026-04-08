using FluentValidation;
using Library.Api.Middleware;
using Library.Application.Abstractions;
using Library.Application.Authors;
using Library.Application.Authors.Dtos;
using Library.Application.Authors.Validation;
using Library.Application.Books;
using Library.Application.Books.Dtos;
using Library.Application.Books.Validation;
using Library.Application.Configuration;
using Library.Infrastructure.Persistence;
using Library.Infrastructure.Repositories;
using Library.Infrastructure.SystemClock;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);

// Logging estructurado (con Trace/Span IDs cuando existan) para facilitar troubleshooting.
builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg
        .ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithSpan()
        .WriteTo.Console();
});

builder.Services.AddControllers();

builder.Services.Configure<LibraryConstraintsOptions>(
    builder.Configuration.GetSection(LibraryConstraintsOptions.SectionName));

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("LibraryDb");
    options.UseSqlServer(cs);
});

builder.Services.AddScoped<ExceptionHandlingMiddleware>();

builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<Library.Application.Abstractions.IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<Library.Application.Abstractions.IBookRepository, BookRepository>();

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddScoped<IValidator<AuthorCreateRequest>, AuthorCreateRequestValidator>();
builder.Services.AddScoped<IValidator<AuthorUpdateRequest>, AuthorUpdateRequestValidator>();
builder.Services.AddScoped<IValidator<BookCreateRequest>, BookCreateRequestValidator>();
builder.Services.AddScoped<IValidator<BookUpdateRequest>, BookUpdateRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library Management Platform API",
        Version = "v1",
        Description = "API de gestión de autores y libros (Clean Architecture, EF Core, SQL Server)."
    });
});

// OpenTelemetry: trazas + métricas (export a consola en Development).
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(serviceName: "Library.Api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        if (builder.Environment.IsDevelopment())
            tracing.AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation();

        if (builder.Environment.IsDevelopment())
            metrics.AddConsoleExporter();
    });

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", p =>
        p.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

// Request logging con Serilog (incluye status code y duración).
app.UseSerilogRequestLogging();

app.UseCors("default");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1");
    c.DocumentTitle = "Library API Docs";
    c.DisplayRequestDuration();
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
