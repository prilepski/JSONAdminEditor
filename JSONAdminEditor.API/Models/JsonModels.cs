using System.ComponentModel.DataAnnotations;

namespace JSONAdminEditor.Models
{
    public class JsonFileViewModel
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? JsonContent { get; set; }
        public List<Dictionary<string, object>>? TableData { get; set; }
        public List<string>? ColumnNames { get; set; }
        public Dictionary<string, string>? ColumnTypes { get; set; } // New property to track column data types
        public string? ErrorMessage { get; set; }
        public bool IsValidJson { get; set; }
        
        // Validation properties
        public string? UniqueField { get; set; }
        public List<ValidationError>? ValidationErrors { get; set; } = new();
    }

    public class FileUploadModel
    {
        [Required]
        public IFormFile? JsonFile { get; set; }
    }

    public class JsonTableRow
    {
        public Dictionary<string, object> Data { get; set; } = new();
        public int RowIndex { get; set; }
    }

    public class ValidationError
    {
        public int RowIndex { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
