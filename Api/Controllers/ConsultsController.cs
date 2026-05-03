using Microsoft.AspNetCore.Mvc;
using SystemSaude.Application.DTOs.Consult;
using SystemSaude.Application.UseCases.Consults;

namespace SystemSaude.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultsController : ControllerBase
{
    private readonly IConsultUseCase _consultUseCase;

    public ConsultsController(IConsultUseCase consultUseCase)
    {
        _consultUseCase = consultUseCase;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ConsultResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ConsultResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var consults = await _consultUseCase.GetAllAsync(cancellationToken);
        return Ok(consults);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ConsultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsultResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var consult = await _consultUseCase.GetByIdAsync(id, cancellationToken);
        if (consult is null)
        {
            return NotFound();
        }

        return Ok(consult);
    }

    [HttpGet("doctor/{doctorId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<ConsultResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ConsultResponse>>> GetByDoctorId(Guid doctorId, CancellationToken cancellationToken)
    {
        var consults = await _consultUseCase.GetByDoctorIdAsync(doctorId, cancellationToken);
        return Ok(consults);
    }

    [HttpGet("pacient/{pacientId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<ConsultResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ConsultResponse>>> GetByPacientId(Guid pacientId, CancellationToken cancellationToken)
    {
        var consults = await _consultUseCase.GetByPacientIdAsync(pacientId, cancellationToken);
        return Ok(consults);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ConsultResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsultResponse>> Create([FromBody] CreateConsultRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var consult = await _consultUseCase.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = consult.Id }, consult);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ConsultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConsultResponse>> Update(Guid id, [FromBody] UpdateConsultRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var consult = await _consultUseCase.UpdateAsync(id, request, cancellationToken);
            if (consult is null)
            {
                return NotFound();
            }

            return Ok(consult);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _consultUseCase.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
