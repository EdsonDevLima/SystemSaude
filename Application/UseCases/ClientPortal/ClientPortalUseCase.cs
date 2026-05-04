using SystemSaude.Application.DTOs.ClientPortal;
using SystemSaude.Domain.Interfaces;

namespace SystemSaude.Application.UseCases.ClientPortal;

public class ClientPortalUseCase : IClientPortalUseCase
{
    private static readonly DayOfWeek[] BusinessDays =
    [
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday
    ];

    private readonly IDoctorRepository _doctorRepository;
    private readonly IPacientRepository _pacientRepository;

    public ClientPortalUseCase(IDoctorRepository doctorRepository, IPacientRepository pacientRepository)
    {
        _doctorRepository = doctorRepository;
        _pacientRepository = pacientRepository;
    }

    public async Task<PacientPortalResponse> RegisterPacientAsync(RegisterPacientRequest request, CancellationToken cancellationToken = default)
    {
        ValidatePacientRequest(request);

        var existingPacient = await _pacientRepository.GetByCpfOrEmailAsync(request.Cpf, request.Email, cancellationToken);
        if (existingPacient is not null)
        {
            return MapPacient(existingPacient);
        }

        var pacient = new Entities.Pacient
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim(),
            Cpf = request.Cpf.Trim(),
            Document = request.Document.Trim(),
            Phone = request.Phone.Trim(),
            Address = new Entities.Address
            {
                id = Guid.NewGuid(),
                street = request.Address.Street.Trim(),
                neighborhood = request.Address.Neighborhood.Trim(),
                state = request.Address.State.Trim(),
                country = request.Address.Country.Trim(),
                complement = request.Address.Complement.Trim()
            }
        };

        await _pacientRepository.AddAsync(pacient, cancellationToken);
        await _pacientRepository.SaveChangesAsync(cancellationToken);

        return MapPacient(pacient);
    }

    public async Task<IReadOnlyList<DoctorOptionResponse>> GetDoctorsAsync(CancellationToken cancellationToken = default)
    {
        var doctors = await _doctorRepository.GetAllAsync(cancellationToken);

        return doctors
            .Where(doctor => doctor.Status)
            .OrderBy(doctor => doctor.Name)
            .Select(doctor => new DoctorOptionResponse
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Speciality = doctor.Speciality,
                Status = doctor.Status
            })
            .ToList();
    }

    public async Task<DoctorAvailabilityResponse?> GetDoctorAvailabilityAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var doctor = await _doctorRepository.GetWithSchedulesAsync(doctorId, cancellationToken);
        if (doctor is null)
        {
            return null;
        }

        var activeConsults = doctor.Consults
            .Where(consult => !string.Equals(consult.Status, "cancelled", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var days = BusinessDays
            .Select(day =>
            {
                var schedule = doctor.Schedules
                    .Where(item => item.DayOfWeek == day && !string.Equals(item.Status, "inactive", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(item => item.StartTime)
                    .FirstOrDefault();

                if (schedule is null)
                {
                    return new DayAvailabilityResponse
                    {
                        DayOfWeek = NormalizeDayOfWeek(day),
                        DayLabel = TranslateDay(day),
                        Slots = []
                    };
                }

                var daySchedules = doctor.Schedules
                    .Where(item => item.DayOfWeek == day && !string.Equals(item.Status, "inactive", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(item => item.StartTime)
                    .ToList();

                var slots = daySchedules
                    .SelectMany(item => BuildSlots(item.StartTime, item.EndTime))
                    .Distinct()
                    .Select(slot => new TimeSlotResponse
                    {
                        StartTime = slot.start.ToString("HH':'mm"),
                        EndTime = slot.end.ToString("HH':'mm"),
                        Available = !activeConsults.Any(consult =>
                            consult.StartAt.DayOfWeek == day &&
                            consult.StartAt.TimeOfDay < slot.end.ToTimeSpan() &&
                            consult.EndAt.TimeOfDay > slot.start.ToTimeSpan())
                    })
                    .ToList();

                return new DayAvailabilityResponse
                {
                    DayOfWeek = NormalizeDayOfWeek(day),
                    DayLabel = TranslateDay(day),
                    Slots = slots
                };
            })
            .ToList();

        return new DoctorAvailabilityResponse
        {
            DoctorId = doctor.Id,
            DoctorName = doctor.Name,
            Speciality = doctor.Speciality,
            Days = days
        };
    }

    private static void ValidatePacientRequest(RegisterPacientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Cpf) ||
            string.IsNullOrWhiteSpace(request.Document) ||
            string.IsNullOrWhiteSpace(request.Phone))
        {
            throw new InvalidOperationException("Preencha os dados principais do paciente.");
        }

        if (string.IsNullOrWhiteSpace(request.Address.Street) ||
            string.IsNullOrWhiteSpace(request.Address.Neighborhood) ||
            string.IsNullOrWhiteSpace(request.Address.State) ||
            string.IsNullOrWhiteSpace(request.Address.Country) ||
            string.IsNullOrWhiteSpace(request.Address.Complement))
        {
            throw new InvalidOperationException("Preencha o endereco completo do paciente.");
        }
    }

    private static PacientPortalResponse MapPacient(Entities.Pacient pacient)
    {
        return new PacientPortalResponse
        {
            Id = pacient.Id,
            Email = pacient.Email,
            Cpf = pacient.Cpf,
            Document = pacient.Document,
            Phone = pacient.Phone,
            Address = new AddressRequest
            {
                Street = pacient.Address.street,
                Neighborhood = pacient.Address.neighborhood,
                State = pacient.Address.state,
                Country = pacient.Address.country,
                Complement = pacient.Address.complement
            }
        };
    }

    private static List<(TimeOnly start, TimeOnly end)> BuildSlots(TimeOnly start, TimeOnly end)
    {
        var slots = new List<(TimeOnly start, TimeOnly end)>();
        var cursor = start;

        while (cursor < end)
        {
            var slotEnd = cursor.AddMinutes(30);
            if (slotEnd > end)
            {
                break;
            }

            slots.Add((cursor, slotEnd));
            cursor = slotEnd;
        }

        return slots;
    }

    private static string TranslateDay(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => "Segunda-feira",
            DayOfWeek.Tuesday => "Terca-feira",
            DayOfWeek.Wednesday => "Quarta-feira",
            DayOfWeek.Thursday => "Quinta-feira",
            DayOfWeek.Friday => "Sexta-feira",
            _ => dayOfWeek.ToString()
        };
    }

    private static int NormalizeDayOfWeek(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Sunday => 7,
            _ => (int)dayOfWeek
        };
    }
}
