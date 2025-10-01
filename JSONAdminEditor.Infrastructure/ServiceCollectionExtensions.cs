using Amazon.S3;
using JSONAdminEditor.Application.Interfaces;
using JSONAdminEditor.Infrastructure.Models;
using JSONAdminEditor.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JSONAdminEditor.Infrastructure;

public static class ServiceCollectionExtensions
{

    public const string StorageSettings = "StorageSettings";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<StorageSettings>(configuration.GetRequiredSection(StorageSettings));

        services.AddAWSService<IAmazonS3>();
        services.AddScoped<FileManagementService>();
        services.AddScoped<S3StorageService>();
        services.AddScoped<IStorageServiceFactory, StorageServiceFactory>();
        services.AddScoped<IFileContentService, FileContentService>();

        return services;
    }
}
