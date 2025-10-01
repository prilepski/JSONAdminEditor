using JSONAdminEditor.Application.Services;
using JSONAdminEditor.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace JSONAdminEditor.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IConfigService, ConfigService>();
        return services;
    }
}
