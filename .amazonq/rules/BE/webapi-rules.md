# ASP.NET Core Web API Rules
## Purpose
To enforce consistent architectural patterns and RESTful design principles for all Web API development.

## Priority
High

## Instructions
-   **Structure REST endpoints logically**: Follow standard REST conventions for URI naming (`/resources/{id}`).
-   **Use controllers for API logic**: Keep business logic out of the controller. Controllers should be lean and delegate to dedicated service or handler classes.
-   **Return standardized API responses**: Return standard `ActionResult` types (e.g., `Ok`, `NotFound`, `BadRequest`) and model-based results, not raw data.
-   **Implement dependency injection correctly**: Use constructor injection for dependencies and ensure services are registered correctly in `Program.cs`.
-   **Handle exceptions globally**: Use middleware to implement a global exception handler instead of using `try-catch` blocks in every controller action.
