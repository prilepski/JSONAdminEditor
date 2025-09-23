using JSONAdminEditor.Services;
using JSONAdminEditor.Models;
using JSONAdminEditor.Middleware;
using JSONAdminEditor.API.Middleware;
using Amazon.S3;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using JSONAdminEditor.API;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Explicitly add Local configuration files to ensure they're loaded
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Local.json", optional: true, reloadOnChange: true);

// Configure storage settings
builder.Services.Configure<StorageSettings>(
    builder.Configuration.GetSection("StorageSettings"));

// Add services to the container.
builder.Services.AddControllers();

// Add modern Microsoft OpenAPI support
builder.Services.AddOpenApi("v1", options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
    options.AddDocumentTransformer<OpenApiDocumentTransformer>();
});

// Add API Explorer for OpenAPI
builder.Services.AddEndpointsApiExplorer();

// Add familiar Swagger UI
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OpenApiDocumentTransformer>();

// Register storage services
builder.Services.AddScoped<FileManagementService>();
builder.Services.AddScoped<S3StorageService>();
builder.Services.AddScoped<IStorageServiceFactory, StorageServiceFactory>();
builder.Services.AddScoped<IFileContentService, FileContentService>();

// Register other services with updated dependencies
builder.Services.AddScoped<IJsonFileService, JsonFileService>();
builder.Services.AddScoped<UniqueFieldValidationService>();
builder.Services.AddScoped<NotificationsService>();
builder.Services.AddScoped<IDataMigrationService, DataMigrationService>();
builder.Services.AddScoped<IBackwardCompatibilityService, BackwardCompatibilityService>();

// Configure AWS S3 client (always register to satisfy dependency injection)
var storageSettings = builder.Configuration.GetSection("StorageSettings").Get<StorageSettings>();
var s3Settings = storageSettings?.S3Settings ?? new S3Settings 
{ 
    Region = "us-east-1", 
    BucketName = "default-bucket", 
    UseCredentialsFromEnvironment = true 
};

// Debug: Show what configuration values are actually loaded
Console.WriteLine("=== Configuration Debug Info ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"StorageType: {storageSettings?.StorageType}");
Console.WriteLine($"BucketName: '{s3Settings.BucketName}'");
Console.WriteLine($"AccessKey: '{s3Settings.AccessKey}'");
Console.WriteLine($"SecretKey length: {s3Settings.SecretKey?.Length ?? 0}");
Console.WriteLine($"UseCredentialsFromEnvironment: {s3Settings.UseCredentialsFromEnvironment}");
Console.WriteLine("===============================");

builder.Services.AddSingleton<IAmazonS3>(provider =>
{
    try
    {
        Console.WriteLine("Creating AWS S3 Client...");
        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
        };
        
        Console.WriteLine($"S3 Region: {s3Settings.Region}");
        Console.WriteLine($"UseCredentialsFromEnvironment: {s3Settings.UseCredentialsFromEnvironment}");
        Console.WriteLine($"AccessKey provided: {!string.IsNullOrEmpty(s3Settings.AccessKey)}");
        Console.WriteLine($"SecretKey provided: {!string.IsNullOrEmpty(s3Settings.SecretKey)}");
        
        // Use explicit credentials if they are provided, otherwise use environment credentials
        if (!string.IsNullOrEmpty(s3Settings.AccessKey) && !string.IsNullOrEmpty(s3Settings.SecretKey))
        {
            Console.WriteLine("Using explicit AWS credentials");
            return new AmazonS3Client(s3Settings.AccessKey, s3Settings.SecretKey, config);
        }
        else if (s3Settings.UseCredentialsFromEnvironment)
        {
            Console.WriteLine("Using environment AWS credentials");
            return new AmazonS3Client(config);
        }
        else
        {
            throw new InvalidOperationException("No AWS credentials configured. Either provide AccessKey/SecretKey or set UseCredentialsFromEnvironment=true");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR creating AWS S3 Client: {ex.GetType().Name}: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw;
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "JSON Admin Editor API v1");
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
        options.EnableValidator();
    });
}

app.UseRouting();

app.UseAuthorization();

// Serve React static files
app.UseStaticFiles();

// Map API controllers
app.MapControllers();

// Serve React app for SPA routes
app.MapFallbackToFile("index.html");

app.Run();
