namespace JSONAdminEditor.Application.Models
{
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
}