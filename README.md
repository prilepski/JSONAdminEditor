# JSON Admin Editor


A .NET 9 web application that provides a web-based interface for editing JSON files. The application displays JSON content in a table format and allows users to modify the data and save changes back to the file.

## Features

- **File Management System**: Organized management of predefined JSON files with structured file types
- **Dual Storage Support**: FileSystem (local) and AWS S3 cloud storage options
- **Secure Credential Management**: Local configuration files for sensitive AWS credentials (excluded from git)
- **Storage Abstraction Layer**: Unified interface supporting seamless switching between storage backends
- **Structured File Types**: Support for Templates, Customers, Event Triggers, Event Channels, and Customer Overrides
- **Vertical Navigation**: Easy navigation between different management pages
- **Dynamic Table Generation**: Automatically converts JSON arrays and objects into editable tables
- **In-line Editing**: Edit data directly in table cells
- **Add/Remove Rows**: Dynamic row management with add and delete functionality
- **Save Changes**: Save modified content back to the original JSON file or S3 bucket
- **Customer-Specific Settings**: Dedicated folder structure for customer configurations
- **File Protection**: Core files are protected from deletion, customer files can be managed
- **Responsive Design**: Bootstrap-based responsive interface
- **Real-time Validation**: Client-side and server-side validation

## Technology Stack

- **.NET 9.0** with ASP.NET Core
- **Razor Pages** for UI
- **AWS SDK for .NET** (AWSSDK.S3, AWSSDK.Extensions.NETCore.Setup)
- **Storage Abstraction Layer** supporting FileSystem and AWS S3
- **Bootstrap 5** for styling
- **JavaScript** for dynamic interactions
- **System.Text.Json & Newtonsoft.Json** for JSON handling
- **Font Awesome** for icons

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio Code (recommended) or Visual Studio

### Installation

1. Clone or download the project
2. Navigate to the project directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```

### Configuration

The application supports both local file system storage and AWS S3 storage for JSON files. For detailed configuration instructions, including:

- Setting up storage types (FileSystem vs S3)
- Configuring AWS credentials securely
- Understanding the file structure and organization
- Setting up local configuration files

**See: [CONFIG_SETUP.md](CONFIG_SETUP.md)** for comprehensive configuration guidance.

### Quick Start Configuration

For immediate development with local file storage (no setup required):
- The application defaults to FileSystem storage
- Files are stored in `wwwroot/data/`
- No additional configuration needed

### Running the Application

#### Using VS Code
- Open the project in VS Code
- Use `Ctrl/Cmd + Shift + P` and run "Tasks: Run Task"
- Select "watch" to run with hot reload, or "run" for a single run

#### Using Command Line
```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Run with hot reload (recommended for development)
dotnet watch run
```

The application will be available at `http://localhost:5000` or `https://localhost:5001`.

## Usage

1. **Upload a JSON File**: 
   - Click "Choose File" and select a `.json` file
   - Click "Upload and Load" to process the file

2. **Edit Data**:
   - Modify values directly in the table cells
   - Add new rows using the "Add Row" button
   - Delete rows using the "Delete" button in each row

3. **Save Changes**:
   - Click "Save Changes" to write modifications back to the file

4. **Load Existing Files**:
   - Use the "Load Existing File" section to work with previously uploaded files

## Project Structure

```
JSONAdminEditor/
├── Models/                    # Data models and ViewModels
│   ├── StorageSettings.cs     # Storage configuration models
│   └── JsonModels.cs         # JSON-related models
├── Pages/                    # Razor Pages
│   ├── Dictionaries.cshtml   # Dictionary management interface
│   ├── ContentVariables.cshtml # Content variables management
│   ├── CustomerEvents.cshtml # Customer events configuration
│   └── Shared/              # Shared layouts and components
├── Services/                 # Business logic and storage abstraction
│   ├── IStorageService.cs    # Storage interface
│   ├── FileManagementService.cs # Local file system storage
│   ├── S3StorageService.cs   # AWS S3 storage implementation
│   ├── StorageServiceFactory.cs # Storage service factory
│   ├── FileContentService.cs # Unified file content access
│   ├── JsonFileService.cs    # JSON file operations
│   └── NotificationsService.cs # Notifications management
├── wwwroot/                  # Static files and local data storage
│   ├── css/                 # Stylesheets
│   ├── js/                  # JavaScript files
│   ├── data/                # JSON data files (FileSystem storage)
│   │   ├── dictionaries/    # Core dictionary files
│   │   ├── customers/       # Customer-specific overrides
│   │   └── notifications.json # Main notifications config
│   └── uploads/             # Temporary uploaded files
├── CONFIG_SETUP.md           # Configuration and storage setup guide
└── Program.cs               # Application configuration
```

## Sample Data

The project includes a sample JSON file (`wwwroot/sample-data.json`) with employee data for testing:

```json
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john.doe@example.com",
    "age": 30,
    "department": "Engineering"
  }
]
```

## Supported JSON Formats

- **JSON Arrays**: Arrays of objects are displayed as tables with each object as a row
- **JSON Objects**: Single objects are converted to single-row tables
- **Nested Objects**: Simple flattening for display (complex nesting may require manual handling)

## Development

### VS Code Tasks

- `build`: Compile the project
- `run`: Run the application once
- `watch`: Run with hot reload for development

### Key Components

- **JsonFileService**: Handles file operations, JSON parsing, and table conversion
- **JsonFileViewModel**: View model for displaying JSON data in table format
- **Index Page**: Main interface with upload, editing, and save functionality

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is open source and available under the MIT License.
