using System.ComponentModel.DataAnnotations;

namespace JSONAdminEditor.Models
{
    public enum FileType
    {
        None = 0,
        Templates = 1,
        Customers = 2,
        CustomerSettings = 4,
        EventTriggers = 5,
        EventChannels = 6,
        OrderTypes = 7
    }

    public class FileManagementViewModel
    {
        public List<ManagedFile> Files { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }

    public class FileUploadViewModel
    {
        public FileType FileType { get; set; } = FileType.None;
        public string? CustomerName { get; set; }
        public string? CustomerIdForFilename { get; set; }
        
        [Required(ErrorMessage = "Please select a JSON file.")]
        public IFormFile? JsonFile { get; set; }
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

    public class ManagedFile
    {
        public string FileName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string DisplayPath { get; set; } = "";
        public FileType FileType { get; set; }
        public string FileTypeDisplay { get; set; } = "";
        public string? CustomerName { get; set; }
        public bool CanDelete { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class Customer
    {
        public string? CustomerId { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Status { get; set; }
    }

    public class CustomerLookupResult
    {
        public string CustomerId { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string MatchType { get; set; } = "";
        public string DisplayText => $"{CompanyName} ({CustomerId})";
    }
}
