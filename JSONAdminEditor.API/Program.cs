
using JSONAdminEditor.API;
using JSONAdminEditor.API.Constants;
using JSONAdminEditor.API.Extensions;
using JSONAdminEditor.API.Middleware;
using JSONAdminEditor.Models;

var builder = WebApplication.CreateBuilder(args);

// Explicitly add Local configuration files to ensure they're loaded
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Local.json", optional: true, reloadOnChange: true);

// Configure storage settings
builder.Services.Configure<StorageSettings>(
    builder.Configuration.GetSection(ConfigurationKeys.StorageSettings));

// Configure Okta settings
//builder.Services.Configure<OktaSettings>(builder.Configuration.GetSection("Okta"));

//// Add authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = "Cookies";
//    options.DefaultChallengeScheme = "oidc";
//})
//.AddCookie("Cookies")
//.AddOpenIdConnect("oidc", options =>
//{
//    var oktaSettings = builder.Configuration.GetSection("Okta").Get<OktaSettings>();
//    options.Authority = oktaSettings?.Domain;
//    options.ClientId = oktaSettings?.ClientId;
//    options.ClientSecret = oktaSettings?.ClientSecret;
//    options.ResponseType = "code";
//    options.SaveTokens = true;
//    options.Scope.Add("openid");
//    options.Scope.Add("profile");
//    options.Scope.Add("email");
//})
;

//builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllers(
//    config =>
//{
//    var policy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//    config.Filters.Add(new AuthorizeFilter(policy));
//}
);

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
builder.Services.AddStorageServices();

// Configure AWS S3 client
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
builder.Services.AddAwsS3Client(builder.Configuration, logger);

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