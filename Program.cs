using JSONAdminEditor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<JsonFileService>();
builder.Services.AddScoped<FileManagementService>();
builder.Services.AddScoped<UniqueFieldValidationService>();
builder.Services.AddScoped<NotificationsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.UseAuthorization();

// Redirect root URL to Dictionaries page since Index was removed
app.MapGet("/", context =>
{
    context.Response.Redirect("/Dictionaries");
    return Task.CompletedTask;
});

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
