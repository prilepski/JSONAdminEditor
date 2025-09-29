namespace JSONAdminEditor.API.Extensions;

using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStorageServices(this IServiceCollection services)
    {
        services.AddScoped<FileManagementService>();
        services.AddScoped<S3StorageService>();
        services.AddScoped<IStorageServiceFactory, StorageServiceFactory>();
        services.AddScoped<IFileContentService, FileContentService>();
        services.AddScoped<IConfigRepository, ConfigRepository>();
        return services;
    }
}