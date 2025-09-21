using System.Text.Json;
using Newtonsoft.Json;
using JSONAdminEditor.Models;

namespace JSONAdminEditor.Services
{
    public class JsonFileService : IJsonFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly UniqueFieldValidationService _validationService;
        private readonly IFileContentService _fileContentService;
        private readonly string _uploadsFolder;

        public JsonFileService(IWebHostEnvironment environment, UniqueFieldValidationService validationService, IFileContentService fileContentService)
        {
            _environment = environment;
            _validationService = validationService;
            _fileContentService = fileContentService;
            _uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            
            // Ensure uploads directory exists (always local for temporary files)
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<JsonFileViewModel> ProcessJsonFileAsync(IFormFile file)
        {
            var model = new JsonFileViewModel
            {
                FileName = file.FileName
            };

            try
            {
                // Save uploaded file
                var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + 
                              Guid.NewGuid().ToString("N")[..8] + 
                              Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_uploadsFolder, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                model.FilePath = filePath;
                return await LoadJsonFileAsync(filePath);
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Error processing file: {ex.Message}";
                model.IsValidJson = false;
                return model;
            }
        }

        public async Task<JsonFileViewModel> LoadJsonFileAsync(string filePath)
        {
            var model = new JsonFileViewModel
            {
                FilePath = filePath,
                FileName = Path.GetFileName(filePath)
            };

            try
            {
                var jsonContent = await _fileContentService.ReadFileAsync(filePath);
                if (string.IsNullOrEmpty(jsonContent))
                {
                    model.ErrorMessage = "File not found or empty";
                    model.IsValidJson = false;
                    return model;
                }
                
                model.JsonContent = jsonContent;

                // Try to parse and convert to table format
                var jsonData = JsonConvert.DeserializeObject(jsonContent);
                
                if (jsonData is Newtonsoft.Json.Linq.JArray jsonArray)
                {
                    model.TableData = ConvertJsonArrayToTable(jsonArray);
                    model.ColumnNames = ExtractColumnNames(jsonArray);
                    model.ColumnTypes = ExtractColumnTypes(jsonArray); // Add column type detection
                    model.IsValidJson = true;
                    
                    // Add uniqueness validation
                    await AddValidationInfoAsync(model, filePath);
                }
                else if (jsonData is Newtonsoft.Json.Linq.JObject jsonObject)
                {
                    // Convert single object to array for table display
                    var singleItemArray = new Newtonsoft.Json.Linq.JArray { jsonObject };
                    model.TableData = ConvertJsonArrayToTable(singleItemArray);
                    model.ColumnNames = ExtractColumnNames(singleItemArray);
                    model.ColumnTypes = ExtractColumnTypes(singleItemArray); // Add column type detection
                    model.IsValidJson = true;
                    
                    // Add uniqueness validation
                    await AddValidationInfoAsync(model, filePath);
                }
                else
                {
                    model.ErrorMessage = "JSON must be an object or array to display as table";
                    model.IsValidJson = false;
                }
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                model.ErrorMessage = $"Invalid JSON format: {ex.Message}";
                model.IsValidJson = false;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Error reading file: {ex.Message}";
                model.IsValidJson = false;
            }

            return model;
        }

        public async Task<bool> SaveJsonFileAsync(string filePath, List<Dictionary<string, object>> tableData)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(tableData, Formatting.Indented);
                return await _fileContentService.WriteFileAsync(filePath, jsonString);
            }
            catch
            {
                return false;
            }
        }

        private List<Dictionary<string, object>> ConvertJsonArrayToTable(Newtonsoft.Json.Linq.JArray jsonArray)
        {
            var tableData = new List<Dictionary<string, object>>();

            foreach (var item in jsonArray)
            {
                var row = new Dictionary<string, object>();
                
                if (item is Newtonsoft.Json.Linq.JObject obj)
                {
                    foreach (var property in obj.Properties())
                    {
                        // Preserve original data types for better handling
                        if (property.Value?.Type == Newtonsoft.Json.Linq.JTokenType.Boolean)
                        {
                            row[property.Name] = (bool)property.Value;
                        }
                        else if (property.Value?.Type == Newtonsoft.Json.Linq.JTokenType.Integer)
                        {
                            row[property.Name] = (int)property.Value;
                        }
                        else if (property.Value?.Type == Newtonsoft.Json.Linq.JTokenType.Float)
                        {
                            row[property.Name] = (decimal)property.Value;
                        }
                        else
                        {
                            row[property.Name] = property.Value?.ToString() ?? "";
                        }
                    }
                }
                
                tableData.Add(row);
            }

            return tableData;
        }

        private List<string> ExtractColumnNames(Newtonsoft.Json.Linq.JArray jsonArray)
        {
            var columns = new HashSet<string>();

            foreach (var item in jsonArray)
            {
                if (item is Newtonsoft.Json.Linq.JObject obj)
                {
                    foreach (var property in obj.Properties())
                    {
                        columns.Add(property.Name);
                    }
                }
            }

            return columns.ToList();
        }

        private Dictionary<string, string> ExtractColumnTypes(Newtonsoft.Json.Linq.JArray jsonArray)
        {
            var columnTypes = new Dictionary<string, string>();

            foreach (var item in jsonArray)
            {
                if (item is Newtonsoft.Json.Linq.JObject obj)
                {
                    foreach (var property in obj.Properties())
                    {
                        var columnName = property.Name;
                        var value = property.Value;

                        // Only set the type if we haven't already determined it or if we need to update it
                        if (!columnTypes.ContainsKey(columnName))
                        {
                            if (value?.Type == Newtonsoft.Json.Linq.JTokenType.Boolean)
                            {
                                columnTypes[columnName] = "boolean";
                            }
                            else if (value?.Type == Newtonsoft.Json.Linq.JTokenType.Integer || 
                                     value?.Type == Newtonsoft.Json.Linq.JTokenType.Float)
                            {
                                columnTypes[columnName] = "number";
                            }
                            else
                            {
                                columnTypes[columnName] = "text";
                            }
                        }
                    }
                }
            }

            return columnTypes;
        }

        public List<string> GetUploadedFiles()
        {
            if (!Directory.Exists(_uploadsFolder))
                return new List<string>();

            return Directory.GetFiles(_uploadsFolder, "*.json")
                           .Select(Path.GetFileName)
                           .Where(name => name != null)
                           .Cast<string>()
                           .ToList();
        }

        public string GetUploadsFolderPath() => _uploadsFolder;
        
        private async Task AddValidationInfoAsync(JsonFileViewModel model, string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            model.UniqueField = _validationService.GetUniqueField(fileName);
            
            if (model.TableData != null && !string.IsNullOrEmpty(model.UniqueField))
            {
                var validationResult = await _validationService.ValidateUniquenessAsync(fileName, model.TableData);
                model.ValidationErrors = validationResult.Errors;
            }
        }
    }
}
