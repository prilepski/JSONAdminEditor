using JSONAdminEditor;
using JSONAdminEditor.Application;
using JSONAdminEditor.Infrastructure;
using JSONAdminEditor.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Explicitly add Local configuration files to ensure they're loaded
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Local.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

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

//app.UseAuthentication();
app.UseAuthorization();

// Serve React static files
app.UseStaticFiles();

// Map API controllers
app.MapControllers();

// Serve React app for SPA routes
app.MapFallbackToFile("index.html");

app.Run();