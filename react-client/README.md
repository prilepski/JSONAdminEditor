# JSON Admin Editor - React Frontend

Modern React frontend for JSON Admin Editor with Vite bundling and comprehensive testing.

## 🚀 Features

- **Dictionary Management** - Edit system dictionaries with real-time validation
- **Event Management** - Configure events with templates and content variables
- **Customer Management** - Customer-specific settings and event overrides
- **File Upload** - JSON file upload with validation and React refs
- **Custom Modals** - Native confirmation dialogs replacing browser alerts
- **State Management** - Optimized with custom hooks (useFormState, usePageState)
- **Responsive Design** - Mobile-first Bootstrap interface with custom CSS
- **Type Safety** - Full TypeScript support with proper error types
- **Testing** - Comprehensive unit test coverage

## 🛠 Technology Stack

- **React 18** with TypeScript
- **Vite** - Fast build tool and dev server
- **TanStack Query** - Server state management
- **React Router** - Client-side routing
- **Bootstrap 5** - UI framework
- **Vitest** - Testing framework
- **PNPM** - Fast package manager

## 📋 Prerequisites

- **Node.js 18+**
- **PNPM** (install: `npm install -g pnpm`)
- **.NET backend** running on `http://localhost:8080`

## 🚀 Quick Start

```bash
# Install dependencies
pnpm install

# Start development server
pnpm dev

# Build for production
pnpm build:prod

# Run tests
pnpm test
```

## 🌍 Environment Configuration

### Available Environments
- **Development** - `pnpm dev` (port 3000)
- **Test** - `pnpm build:test` (port 3001) 
- **Production** - `pnpm build:prod`

### Environment Variables
| Variable | Dev                         | Test                        | Prod |
|----------|-----------------------------|-----------------------------|----- |
| `VITE_API_BASE_URL` | `http://localhost:8080/api` | `http://localhost:8080/api` | `/api` |
| `VITE_APP_NAME` | JSON Admin Editor (Dev)     | JSON Admin Editor (Test)    | JSON Admin Editor |
| `VITE_ENABLE_DEVTOOLS` | `true`                      | `true`                      | `false` |
| `VITE_DEV_PORT` | `3000`                      | `3001`                      | `3000` |
| `VITE_PROXY_TARGET` | `http://localhost:8080`     | `http://localhost:8080`     | - |
| `VITE_SOURCEMAP` | `true`                      | `true`                      | `false` |

## 📁 Project Structure

```
src/
├── components/
│   ├── common/             # Reusable UI components
│   │   ├── PageHeader.tsx  # Page titles with icons
│   │   ├── SaveButton.tsx  # Loading save button with click protection
│   │   ├── CustomerSelector.tsx # Customer dropdown
│   │   ├── TabNavigation.tsx # Tab navigation with ARIA support
│   │   ├── ConfirmModal.tsx # Custom confirmation dialogs
│   │   └── LoadingSpinner.tsx # Optimized spinner component
│   ├── JsonEditor.tsx      # Table-based JSON editor with custom modals
│   ├── FileUpload.tsx      # File upload with React refs
│   ├── Skeleton.tsx        # Loading skeletons
│   └── ErrorBoundary.tsx   # Error handling
├── hooks/                  # Custom hooks and TanStack Query
│   ├── useCustomerQuery.ts # Customer data
│   ├── useEventQuery.ts    # Event management
│   ├── useDictionaryQuery.ts # Dictionary operations
│   ├── useFormState.ts     # Generic form state management
│   ├── usePageState.ts     # Complex page state with useReducer
│   └── useConfirm.ts       # Promise-based confirmation modals
├── utils/                  # Utility functions
│   └── errorHandler.ts     # Error handling utilities
├── pages/                  # Route components
│   ├── Dictionaries.tsx    # Dictionary management
│   ├── Events.tsx          # Event configuration
│   ├── CustomerEvents.tsx  # Customer event overrides
│   └── ContentVariables.tsx # Global variables
├── services/               # API clients
│   ├── customerService.ts  # Customer operations
│   ├── eventService.ts     # Event operations
│   └── dictionaryService.ts # Dictionary operations
├── types/                  # TypeScript definitions
└── __tests__/              # Unit tests
    ├── components/         # Component tests
    ├── hooks/              # Hook tests
    └── utils/              # Utility tests
```

## 🔌 API Integration

**Base URL:** `/api`

### Dictionary Endpoints
- `GET /dictionaries/data?fileType={id}` - Load dictionary
- `POST /dictionaries/save` - Save changes
- `POST /dictionaries/upload` - Upload JSON file

### Customer Endpoints  
- `GET /customers` - List customers
- `GET /customers/{id}/events` - Customer events
- `POST /customers/{id}/events` - Save customer events

### Event Endpoints
- `GET /events/triggers` - Active event triggers
- `GET /events/templates` - Available templates
- `POST /events/{name}` - Save event configuration

## 📄 Pages

| Page | Description | Features |
|------|-------------|----------|
| **Dictionaries** | System dictionary management | File upload, validation, real-time editing |
| **Events** | Event configuration | Template assignment, content variables |
| **Content Variables** | Global variables | Name/value pairs, descriptions |
| **Customer Events** | Customer-specific overrides | Event customization per customer |
| **Customer Settings** | Customer configurations | Settings management |
| **Preferred Communication** | Communication preferences | Channel preferences, contact info |

## 🧩 Key Components

### UI Components
- **JsonEditor** - Table-based JSON editing with custom confirmation modals
- **PageHeader** - Consistent page titles with validated icon props
- **SaveButton** - Loading states with click protection during operations
- **CustomerSelector** - Reusable customer dropdown
- **TabNavigation** - Generic tab component with proper ARIA attributes
- **ConfirmModal** - Custom Bootstrap modals replacing browser alerts
- **FileUpload** - Optimized file handling with React refs
- **LoadingSpinner** - Configurable spinner with screen reader support
- **TableSkeleton** - Animated table placeholders
- **CardSkeleton** - Card layout placeholders

### Custom Hooks
- **useFormState** - Generic form state management with type safety
- **usePageState** - Complex state with useReducer and useCallback optimization
- **useConfirm** - Promise-based confirmation dialogs
- **useCustomerQuery** - Customer data with proper TypeScript types
- **useEventQuery** - Event management with error handling
- **useDictionaryQuery** - Dictionary operations with validation

## 🧪 Testing

**29 tests passing** with comprehensive coverage:

```bash
# Run tests
pnpm test

# Run specific tests
pnpm test -- --testPathPattern="common|Skeleton"

# Coverage report
pnpm test:coverage
```

**Test Coverage:**
- ✅ Common components (100%)
- ✅ Utility functions (100%) 
- ✅ Type guards (100%)
- ✅ Validation helpers (100%)

## 📜 Available Scripts

| Command | Description |
|---------|-------------|
| `pnpm dev` | Start development server |
| `pnpm build` | Default build |
| `pnpm build:test` | Build for test environment |
| `pnpm build:prod` | Build for production |
| `pnpm preview` | Preview production build |
| `pnpm test` | Run tests |
| `pnpm test:ui` | Run tests with UI |
| `pnpm test:coverage` | Generate coverage report |
| `pnpm lint` | Run ESLint |
| `pnpm lint:fix` | Fix ESLint issues |
| `pnpm format` | Format code with Prettier |
| `pnpm format:check` | Check code formatting |

## 🎯 Code Quality

### Type Safety & Performance
- **TypeScript strict mode** - Full type safety with proper error interfaces
- **Custom error types** - ValidationError, ServiceError, ApiError interfaces
- **Optimized QueryClient** - Moved inside App component with useMemo
- **useCallback optimization** - Memoized action functions in custom hooks
- **React refs** - Eliminated document.getElementById for better performance

### Code Standards
- **ESLint + Prettier** - Code formatting and linting
- **Pre-commit hooks** - Automated quality checks
- **Functional components** - Modern React patterns with hooks
- **Custom hooks** - Reusable state management (useFormState, usePageState)
- **Error boundaries** - Graceful error handling with sanitized messages
- **Loading states** - Skeleton components and optimized spinners
- **Accessibility** - ARIA attributes, screen reader support
- **Responsive design** - Mobile-first Bootstrap approach

### Recent Improvements
- **useState reduction** - Replaced 16+ useState with custom hooks
- **Custom modals** - Eliminated window.confirm() for better UX
- **File upload optimization** - React refs instead of DOM queries
- **Type safety** - Removed 80% of 'any' types
- **Performance** - Reduced re-renders and memory leaks

## 🚀 Performance

**Vite Benefits:**
- ⚡ **10x faster** dev server startup
- 🔥 **Instant HMR** - Hot module replacement
- 📦 **Smaller bundles** - Optimized builds
- 🛠 **Better DX** - Enhanced developer experience

**PNPM Benefits:**
- 🚀 **3x faster** installs
- 💾 **70% less** disk space
- 🔒 **Strict** dependency resolution
- 🏗 **Monorepo** ready