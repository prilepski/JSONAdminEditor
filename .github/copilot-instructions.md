# Copilot Instructions for JSON Admin Editor

<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

## Project Overview
This is a .NET 9 web application that provides a web-based interface for editing JSON files. The application displays JSON content in a table format and allows users to modify the data and save changes back to the file.

## Key Features
- JSON file upload and selection
- Dynamic table generation from JSON data
- In-line editing capabilities
- Save modified content back to JSON files
- Responsive web interface

## Technology Stack
- .NET 9.0 with ASP.NET Core
- Razor Pages for UI
- Bootstrap for styling
- JavaScript for dynamic interactions
- System.Text.Json for JSON handling

## Code Style Guidelines
- Use async/await patterns for file operations
- Follow ASP.NET Core conventions for Razor Pages
- Use proper error handling and validation
- Implement responsive design principles
- Use semantic HTML and accessible UI components

## Project Structure
- `/Pages` - Razor Pages for UI
- `/Models` - Data models and ViewModels
- `/Services` - Business logic and file operations
- `/wwwroot` - Static files (CSS, JS, images)
