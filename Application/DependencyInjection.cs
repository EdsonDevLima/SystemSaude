using SystemSaude.Application.UseCases.Consults;

namespace SystemSaude.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IConsultUseCase, ConsultUseCase>();
        return services;
    }
}
