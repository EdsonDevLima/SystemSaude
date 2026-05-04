using SystemSaude.Application.UseCases.Consults;
using SystemSaude.Application.UseCases.ClientPortal;

namespace SystemSaude.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IConsultUseCase, ConsultUseCase>();
        services.AddScoped<IClientPortalUseCase, ClientPortalUseCase>();
        return services;
    }
}
