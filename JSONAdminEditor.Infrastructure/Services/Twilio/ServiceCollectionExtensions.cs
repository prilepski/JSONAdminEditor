
using JSONAdminEditor.Infrastructure.Services.Twilio.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JSONAdminEditor.Infrastructure.Services.Twilio;

public static class ServiceCollectionExtensions
{

    public const string ConfigurationKey = "Twilio";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<Configuration>(configuration.GetRequiredSection(ConfigurationKey));

        return services;
    }
}
