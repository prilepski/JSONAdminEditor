using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace JSONAdminEditor.Application.Models
{
    public class FileUploadViewModel
    {
        public FileType FileType { get; set; } = FileType.None;
        public string? CustomerName { get; set; }
        public string? CustomerIdForFilename { get; set; }
        
        [Required(ErrorMessage = "Please select a JSON file.")]
        public IFormFile? JsonFile { get; set; }
    }
}