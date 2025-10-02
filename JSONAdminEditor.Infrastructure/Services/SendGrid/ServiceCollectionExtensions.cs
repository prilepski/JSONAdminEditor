using JSONAdminEditor.Infrastructure.Services.SendGrid.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JSONAdminEditor.Infrastructure.Services.SendGrid;

public static class ServiceCollectionExtensions
{

    public const string ConfigurationKey = "SendGrid";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<Configuration>(configuration.GetRequiredSection(ConfigurationKey));

        return services;
    }
}
