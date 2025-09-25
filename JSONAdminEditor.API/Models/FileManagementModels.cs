using System.ComponentModel.DataAnnotations;
using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Models;

public class FileManagementViewModel
{
    public List<ManagedFile> Files { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}

public class CustomerSettingsUploadViewModel
{
    public string? CustomerName { get; set; }
    public string? CustomerIdForFilename { get; set; }
    
    [Required(ErrorMessage = "Please select a JSON file.")]
    public IFormFile? JsonFile { get; set; }
    
    // FileType is always CustomerSettings, so no need to expose it
    public FileType FileType => FileType.CustomerSettings;
}

public class CustomerLookupResult
{
    public string CustomerId { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string MatchType { get; set; } = "";
    public string DisplayText => $"{CompanyName} ({CustomerId})";
}
