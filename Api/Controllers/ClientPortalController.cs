using Microsoft.AspNetCore.Mvc;
using SystemSaude.Application.DTOs.ClientPortal;
using SystemSaude.Application.UseCases.ClientPortal;

namespace SystemSaude.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientPortalController : ControllerBase
{
    private readonly IClientPortalUseCase _clientPortalUseCase;

    public ClientPortalController(IClientPortalUseCase clientPortalUseCase)
    {
        _clientPortalUseCase = clientPortalUseCase;
    }

    [HttpPost("pacients/register")]
    [ProducesResponseType(typeof(PacientPortalResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PacientPortalResponse>> RegisterPacient(
        [FromBody] RegisterPacientRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var pacient = await _clientPortalUseCase.RegisterPacientAsync(request, cancellationToken);
            return CreatedAtAction(nameof(RegisterPacient), new { id = pacient.Id }, pacient);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("doctors")]
    [ProducesResponseType(typeof(IReadOnlyList<DoctorOptionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DoctorOptionResponse>>> GetDoctors(CancellationToken cancellationToken)
    {
        var doctors = await _clientPortalUseCase.GetDoctorsAsync(cancellationToken);
        return Ok(doctors);
    }

    [HttpGet("doctors/{doctorId:guid}/availability")]
    [ProducesResponseType(typeof(DoctorAvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoctorAvailabilityResponse>> GetDoctorAvailability(Guid doctorId, CancellationToken cancellationToken)
    {
        var availability = await _clientPortalUseCase.GetDoctorAvailabilityAsync(doctorId, cancellationToken);
        if (availability is null)
        {
            return NotFound();
        }

        return Ok(availability);
    }
}
