# Clean Architecture Rules
## Purpose
To enforce Clean Architecture principles, ensuring a clear separation of concerns between layers.

## Priority
Critical

## Instructions
-   **Follow Dependency Rule**: All dependencies must point inward. Application Core layers (e.g., `Domain`, `Application`) must not know about external concerns (e.g., `Infrastructure`, `Presentation`).
-   **Isolate Domain Logic**: Place all business logic and entities within the `Domain` project. The `Application` layer should contain use cases and orchestrate the domain.
-   **Handle Infrastructure Externally**: Databases, external services, and third-party APIs should only be referenced within the `Infrastructure` project. Use interfaces defined in the `Application` layer to connect to them.
-   **Separate Data Transfer Objects (DTOs)**: Ensure DTOs are distinct from domain models. Use mapping (e.g., with libraries like AutoMapper) to transfer data between layers.
-   **Enforce Inversion of Control**: Use dependency injection to manage dependencies. Define interfaces in the core layers and implement them in the external layers.
