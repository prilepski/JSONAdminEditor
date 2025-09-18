# JSON Admin Editor - React Frontend

Modern React frontend for JSON Admin Editor with Vite bundling and comprehensive testing.

## 🚀 Features

- **Dictionary Management** - Edit system dictionaries with real-time validation
- **Event Management** - Configure events with templates and content variables
- **Customer Management** - Customer-specific settings and event overrides
- **File Upload** - JSON file upload with validation
- **Responsive Design** - Mobile-first Bootstrap interface
- **Type Safety** - Full TypeScript support
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
- **.NET backend** running on `http://localhost:5000`

## 🚀 Quick Start

```bash
# Install dependencies
npm install
# OR
pnpm install

# Start development server
npm run dev
# OR
pnpm dev

# Build for production
npm run build:prod
# OR
pnpm build:prod

# Run tests
npm test
# OR
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
| `VITE_API_BASE_URL` | `http://localhost:5000/api` | `http://localhost:5000/api` | `/api` |
| `VITE_APP_NAME` | JSON Admin Editor (Dev)     | JSON Admin Editor (Test)    | JSON Admin Editor |
| `VITE_ENABLE_DEVTOOLS` | `true`                      | `true`                      | `false` |
| `VITE_DEV_PORT` | `3000`                      | `3001`                      | `3000` |
| `VITE_PROXY_TARGET` | `http://localhost:5000`     | `http://localhost:5001`     | - |
| `VITE_SOURCEMAP` | `true`                      | `true`                      | `false` |

## 📁 Project Structure

```
src/
├── components/
│   ├── common/             # Reusable UI components
│   │   ├── PageHeader.tsx  # Page titles with icons
│   │   ├── SaveButton.tsx  # Loading save button
│   │   ├── CustomerSelector.tsx # Customer dropdown
│   │   └── TabNavigation.tsx # Tab navigation
│   ├── JsonEditor.tsx      # Table-based JSON editor
│   ├── Skeleton.tsx        # Loading skeletons
│   └── ErrorBoundary.tsx   # Error handling
├── hooks/                  # TanStack Query hooks
│   ├── useCustomerQuery.ts # Customer data
│   ├── useEventQuery.ts    # Event management
│   └── useDictionaryQuery.ts # Dictionary operations
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

- **JsonEditor** - Table-based JSON editing with validation
- **PageHeader** - Consistent page titles with icons
- **SaveButton** - Loading states and error handling
- **CustomerSelector** - Reusable customer dropdown
- **TabNavigation** - Generic tab component
- **LoadingSpinner** - Configurable loading spinner
- **TableSkeleton** - Animated table placeholders
- **CardSkeleton** - Card layout placeholders

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

- **TypeScript strict mode** - Full type safety
- **ESLint + Prettier** - Code formatting and linting
- **Pre-commit hooks** - Automated quality checks
- **Functional components** - Modern React patterns
- **Custom hooks** - Reusable logic
- **Error boundaries** - Graceful error handling
- **Loading states** - Skeleton components and spinners
- **Responsive design** - Mobile-first approach

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