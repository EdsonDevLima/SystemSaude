using SystemSaude.Application.DTOs.Consult;
using SystemSaude.Domain.Interfaces;
using SystemSaude.Entities;

namespace SystemSaude.Application.UseCases.Consults;

public class ConsultUseCase : IConsultUseCase
{
    private readonly IConsultRepository _consultRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPacientRepository _pacientRepository;

    public ConsultUseCase(
        IConsultRepository consultRepository,
        IDoctorRepository doctorRepository,
        IPacientRepository pacientRepository)
    {
        _consultRepository = consultRepository;
        _doctorRepository = doctorRepository;
        _pacientRepository = pacientRepository;
    }

    public async Task<IReadOnlyList<ConsultResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var consults = await _consultRepository.GetAllAsync(cancellationToken);
        return consults.Select(MapToResponse).ToList();
    }

    public async Task<ConsultResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var consult = await _consultRepository.GetByIdAsync(id, cancellationToken);
        return consult is null ? null : MapToResponse(consult);
    }

    public async Task<IReadOnlyList<ConsultResponse>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var consults = await _consultRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
        return consults.Select(MapToResponse).ToList();
    }

    public async Task<IReadOnlyList<ConsultResponse>> GetByPacientIdAsync(Guid pacientId, CancellationToken cancellationToken = default)
    {
        var consults = await _consultRepository.GetByPacientIdAsync(pacientId, cancellationToken);
        return consults.Select(MapToResponse).ToList();
    }

    public async Task<ConsultResponse> CreateAsync(CreateConsultRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateRequestAsync(request.DoctorId, request.PacientId, request.StartAt, request.EndAt, cancellationToken);

        var consult = new Consult
        {
            Id = Guid.NewGuid(),
            DoctorId = request.DoctorId,
            PatientId = request.PacientId,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "scheduled" : request.Status,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _consultRepository.AddAsync(consult, cancellationToken);
        await _consultRepository.SaveChangesAsync(cancellationToken);

        consult.Doctor = (await _doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken))!;
        consult.Pacient = (await _pacientRepository.GetByIdAsync(request.PacientId, cancellationToken))!;

        return MapToResponse(consult);
    }

    public async Task<ConsultResponse?> UpdateAsync(Guid id, UpdateConsultRequest request, CancellationToken cancellationToken = default)
    {
        var consult = await _consultRepository.GetByIdAsync(id, cancellationToken);
        if (consult is null)
        {
            return null;
        }

        if (request.StartAt >= request.EndAt)
        {
            throw new InvalidOperationException("A data final da consulta deve ser maior que a inicial.");
        }

        consult.StartAt = request.StartAt;
        consult.EndAt = request.EndAt;
        consult.Status = string.IsNullOrWhiteSpace(request.Status) ? consult.Status : request.Status;
        consult.Notes = request.Notes;

        _consultRepository.Update(consult);
        await _consultRepository.SaveChangesAsync(cancellationToken);

        consult.Doctor ??= (await _doctorRepository.GetByIdAsync(consult.DoctorId, cancellationToken))!;
        consult.Pacient ??= (await _pacientRepository.GetByIdAsync(consult.PatientId, cancellationToken))!;

        return MapToResponse(consult);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var consult = await _consultRepository.GetByIdAsync(id, cancellationToken);
        if (consult is null)
        {
            return false;
        }

        _consultRepository.Remove(consult);
        await _consultRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task ValidateRequestAsync(
        Guid doctorId,
        Guid pacientId,
        DateTime startAt,
        DateTime endAt,
        CancellationToken cancellationToken)
    {
        if (startAt >= endAt)
        {
            throw new InvalidOperationException("A data final da consulta deve ser maior que a inicial.");
        }

        var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
        if (doctor is null)
        {
            throw new KeyNotFoundException("Medico nao encontrado.");
        }

        var pacient = await _pacientRepository.GetByIdAsync(pacientId, cancellationToken);
        if (pacient is null)
        {
            throw new KeyNotFoundException("Paciente nao encontrado.");
        }
    }

    private static ConsultResponse MapToResponse(Consult consult)
    {
        return new ConsultResponse
        {
            Id = consult.Id,
            DoctorId = consult.DoctorId,
            DoctorName = consult.Doctor?.Name ?? string.Empty,
            PacientId = consult.PatientId,
            PacientEmail = consult.Pacient?.Email ?? string.Empty,
            StartAt = consult.StartAt,
            EndAt = consult.EndAt,
            Status = consult.Status,
            Notes = consult.Notes,
            CreatedAt = consult.CreatedAt
        };
    }
}
