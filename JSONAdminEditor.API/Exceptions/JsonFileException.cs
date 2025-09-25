namespace JSONAdminEditor.API.Exceptions
{
    public class JsonFileException : Exception
    {
        public JsonFileException(string message) : base(message) { }
        
        public JsonFileException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class StorageException : Exception
    {
        public StorageException(string message) : base(message) { }
        
        public StorageException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
        
        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }
}