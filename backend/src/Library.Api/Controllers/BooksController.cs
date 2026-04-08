using Library.Api.Contracts;
using Library.Application.Books;
using Library.Application.Books.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _books;

    public BooksController(IBookService books)
    {
        _books = books;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<BookResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<BookResponse>>>> List(CancellationToken ct)
    {
        // Controlador thin: sin acceso a DB, sin reglas; solo delega a Application.
        var data = await _books.ListAsync(ct);
        return Ok(new ApiResponse<List<BookResponse>>
        {
            Success = true,
            Message = "OK",
            Data = data,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Get(Guid id, CancellationToken ct)
    {
        var data = await _books.GetAsync(id, ct);
        return Ok(new ApiResponse<BookResponse>
        {
            Success = true,
            Message = "OK",
            Data = data,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Create([FromBody] BookCreateRequest request, CancellationToken ct)
    {
        // El controlador es thin: delega reglas y validación al caso de uso (Application).
        var created = await _books.CreateAsync(request, ct);

        return CreatedAtAction(nameof(Get), new { id = created.Id }, new ApiResponse<BookResponse>
        {
            Success = true,
            Message = "Libro creado.",
            Data = created,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<BookResponse>>> Update(Guid id, [FromBody] BookUpdateRequest request, CancellationToken ct)
    {
        // Reglas del enunciado (autor existente, máximo libros) viven en Application/Domain.
        var updated = await _books.UpdateAsync(id, request, ct);
        return Ok(new ApiResponse<BookResponse>
        {
            Success = true,
            Message = "Libro actualizado.",
            Data = updated,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await _books.DeleteAsync(id, ct);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Libro eliminado.",
            Data = null,
            TraceId = HttpContext.TraceIdentifier
        });
    }
}

