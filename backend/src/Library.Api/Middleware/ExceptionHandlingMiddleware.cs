using System.Net;
using Library.Api.Contracts;
using Library.Application.Common;
using Library.Domain.Common;

namespace Library.Api.Middleware;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            // Validación de request DTO (FluentValidation) -> 400 con detalles.
            await WriteAsync(context, HttpStatusCode.BadRequest, ex.Message, ex.Errors);
        }
        catch (NotFoundException ex)
        {
            // Recurso inexistente -> 404.
            await WriteAsync(context, HttpStatusCode.NotFound, ex.Message, Array.Empty<string>());
        }
        catch (ConflictException ex)
        {
            // Conflictos de negocio (ej. email duplicado) -> 409.
            await WriteAsync(context, HttpStatusCode.Conflict, ex.Message, Array.Empty<string>());
        }
        catch (DomainException ex)
        {
            // Mensajes de negocio obligatorios (exactos) vienen desde Domain.
            // Se propagan “tal cual” para cumplir el enunciado.
            await WriteAsync(context, HttpStatusCode.BadRequest, ex.Message, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            // Fallback seguro: no filtramos detalles internos.
            _logger.LogError(ex, "Unhandled exception");
            await WriteAsync(context, HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.", Array.Empty<string>());
        }
    }

    private static async Task WriteAsync(HttpContext context, HttpStatusCode status, string message, IReadOnlyCollection<string> errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var traceId = context.TraceIdentifier;

        var payload = new ApiErrorResponse
        {
            Success = false,
            Message = message,
            Errors = errors,
            TraceId = traceId
        };

        await context.Response.WriteAsJsonAsync(payload);
    }
}

