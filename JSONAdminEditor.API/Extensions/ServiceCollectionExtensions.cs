namespace JSONAdminEditor.API.Extensions
{
    using JSONAdminEditor.API.Constants;
    using JSONAdminEditor.Application.Interfaces;
    using JSONAdminEditor.Models;
    using JSONAdminEditor.Services;
    using Amazon.S3;
    using Amazon;
    using Microsoft.Extensions.Options;

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

        public static IServiceCollection AddAwsS3Client(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var storageSettings = configuration.GetSection(ConfigurationKeys.StorageSettings).Get<StorageSettings>();
            var s3Settings = storageSettings?.S3Settings ?? new S3Settings 
            { 
                Region = "us-east-1", 
                BucketName = "default-bucket", 
                UseCredentialsFromEnvironment = true 
            };

            services.AddSingleton<IAmazonS3>(provider => CreateS3Client(s3Settings, logger));
            return services;
        }

        private static IAmazonS3 CreateS3Client(S3Settings s3Settings, ILogger logger)
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
            };

            if (!string.IsNullOrEmpty(s3Settings.AccessKey) && !string.IsNullOrEmpty(s3Settings.SecretKey))
            {
                logger.LogInformation("Using explicit AWS credentials");
                return new AmazonS3Client(s3Settings.AccessKey, s3Settings.SecretKey, config);
            }
            
            if (s3Settings.UseCredentialsFromEnvironment)
            {
                logger.LogInformation("Using environment AWS credentials");
                return new AmazonS3Client(config);
            }
            
            throw new InvalidOperationException("No AWS credentials configured. Either provide AccessKey/SecretKey or set UseCredentialsFromEnvironment=true");
        }
    }
}