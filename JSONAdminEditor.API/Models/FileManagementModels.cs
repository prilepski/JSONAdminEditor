using JSONAdminEditor.Application.Models;

namespace JSONAdminEditor.Models;

public class FileManagementViewModel
{
    public List<ManagedFile> Files { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
