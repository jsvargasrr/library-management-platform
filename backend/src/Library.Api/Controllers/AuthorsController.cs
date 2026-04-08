using Library.Api.Contracts;
using Library.Application.Authors;
using Library.Application.Authors.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/authors")]
public sealed class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authors;

    public AuthorsController(IAuthorService authors)
    {
        _authors = authors;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AuthorResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AuthorResponse>>>> List(CancellationToken ct)
    {
        // Controlador thin: delega todo el comportamiento en Application.
        var data = await _authors.ListAsync(ct);
        return Ok(new ApiResponse<List<AuthorResponse>>
        {
            Success = true,
            Message = "OK",
            Data = data,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AuthorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AuthorResponse>>> Get(Guid id, CancellationToken ct)
    {
        var data = await _authors.GetAsync(id, ct);
        return Ok(new ApiResponse<AuthorResponse>
        {
            Success = true,
            Message = "OK",
            Data = data,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AuthorResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AuthorResponse>>> Create([FromBody] AuthorCreateRequest request, CancellationToken ct)
    {
        // Validación (DTO + negocio) y conflictos (email único) se manejan en Application + middleware global.
        var created = await _authors.CreateAsync(request, ct);

        return CreatedAtAction(nameof(Get), new { id = created.Id }, new ApiResponse<AuthorResponse>
        {
            Success = true,
            Message = "Autor creado.",
            Data = created,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AuthorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AuthorResponse>>> Update(Guid id, [FromBody] AuthorUpdateRequest request, CancellationToken ct)
    {
        var updated = await _authors.UpdateAsync(id, request, ct);
        return Ok(new ApiResponse<AuthorResponse>
        {
            Success = true,
            Message = "Autor actualizado.",
            Data = updated,
            TraceId = HttpContext.TraceIdentifier
        });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        // Se impide eliminar autores con libros asociados (integridad referencial + regla de negocio).
        await _authors.DeleteAsync(id, ct);
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Autor eliminado.",
            Data = null,
            TraceId = HttpContext.TraceIdentifier
        });
    }
}

