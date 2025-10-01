using Amazon.S3;
using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JSONAdminEditor.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<FileManagementService>();
        services.AddScoped<S3StorageService>();
        services.AddScoped<IStorageServiceFactory, StorageServiceFactory>();
        services.AddScoped<IFileContentService, FileContentService>();
        return services;
    }
}
