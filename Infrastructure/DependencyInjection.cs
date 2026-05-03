using SystemSaude.Domain.Interfaces;
using SystemSaude.Infrastructure.Repositories;

namespace SystemSaude.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IConsultRepository, ConsultRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPacientRepository, PacientRepository>();
        services.AddScoped<ISchedulesRepository, SchedulesRepository>();

        return services;
    }
}
