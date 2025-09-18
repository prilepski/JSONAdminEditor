# JSON Admin Editor - React Frontend

This is the React frontend for the JSON Admin Editor application, rewritten from Razor Pages to provide a modern, responsive user interface.

## Features

- **Dictionary Management**: Edit system dictionaries (Templates, Event Triggers, Event Channels, Order Types, Customers)
- **Real-time Editing**: In-line table editing with validation
- **File Upload**: Replace dictionaries with JSON file uploads
- **Responsive Design**: Bootstrap-based responsive interface
- **TypeScript**: Full TypeScript support for type safety
- **Modern React**: Uses React 18 with hooks and functional components

## Technology Stack

- **React 18** with TypeScript
- **React Router** for navigation
- **Bootstrap 5** for styling
- **Axios** for API communication
- **Font Awesome** for icons

## Getting Started

### Prerequisites

- Node.js 16+ and npm
- .NET 9 backend running on `http://localhost:5000`

### Installation

1. Navigate to the react-client directory:
   ```bash
   cd react-client
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

The React app will be available at `http://localhost:3000` and will proxy API requests to the .NET backend at `http://localhost:5000`.

### Building for Production

1. Build the React app:
   ```bash
   npm run build
   ```

2. Copy the build files to the .NET wwwroot directory:
   ```bash
   cp -r build/* ../wwwroot/
   ```

## Project Structure

```
react-client/
├── src/
│   ├── components/          # Reusable React components
│   │   ├── AlertMessage.tsx # Alert notifications
│   │   ├── DictionarySelector.tsx # Dictionary type selector
│   │   ├── FileUpload.tsx   # File upload with confirmation
│   │   └── JsonEditor.tsx   # Table editor for JSON data
│   ├── hooks/              # Custom React hooks
│   │   └── useDictionary.ts # Dictionary management hook
│   ├── pages/              # Page components
│   │   └── Dictionaries.tsx # Main dictionaries page
│   ├── services/           # API services
│   │   └── api.ts          # Axios-based API client
│   ├── types/              # TypeScript type definitions
│   │   └── index.ts        # Shared types and interfaces
│   ├── App.tsx             # Main app component with routing
│   ├── App.css             # Application styles
│   ├── index.tsx           # React entry point
│   └── index.css           # Base styles
├── public/
│   └── index.html          # HTML template
├── package.json            # Dependencies and scripts
└── tsconfig.json           # TypeScript configuration
```

## API Integration

The React frontend communicates with the .NET backend through REST API endpoints:

- `GET /api/dictionaries/data?fileType={id}` - Load dictionary data
- `POST /api/dictionaries/save` - Save dictionary changes
- `POST /api/dictionaries/upload` - Upload new dictionary file

## Pages

### Dictionaries
Manage system dictionaries (Templates, Event Triggers, Event Channels, Order Types, Customers)
- Dictionary selection and editing
- File upload with confirmation
- Real-time validation

### Events
Event management with support for order-type variants
- Event data editing
- Template assignment
- Content variables management
- Order type support detection

### Content Variables
Global content variables management
- Variable name/value pairs
- Description fields
- Uniqueness validation

### Customer Events
Customer-specific event configurations
- Customer selection
- Event overrides per customer

### Customer Settings
Customer-specific settings management
- Customer selection
- Settings configuration

### Preferred Communication
Customer communication preferences
- Preferred channels
- Contact information
- Active status management

## Key Components

### DictionarySelector
Dropdown component for selecting dictionary types

### JsonEditor
Table-based JSON editing with validation and type-aware controls

### FileUpload
File upload with confirmation modals

### AlertMessage
Notification system for success/error messages

### useDictionary Hook
Custom hook for dictionary operations

## Validation

The frontend includes:
- Client-side validation for required fields
- Real-time validation error display
- Server-side validation integration
- Unique field validation for dictionary entries

## Responsive Design

The interface is fully responsive with:
- Mobile-friendly table scrolling
- Collapsible navigation
- Adaptive form layouts
- Touch-friendly controls

## Development

### Available Scripts

- `npm start` - Start development server
- `npm run build` - Build for production
- `npm test` - Run tests
- `npm run eject` - Eject from Create React App

### Code Style

The project uses:
- TypeScript strict mode
- ESLint with React rules
- Functional components with hooks
- Modern ES6+ syntax

## Migration from Razor Pages

This React version provides the same functionality as the original Razor Pages application with these improvements:

- **Better User Experience**: No page reloads, instant feedback
- **Modern Architecture**: Component-based, reusable code
- **Type Safety**: Full TypeScript integration
- **Performance**: Client-side rendering and caching
- **Maintainability**: Cleaner separation of concerns

The backend API maintains compatibility with the existing storage and validation services.