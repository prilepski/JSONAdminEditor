# C# Code Styling
## Purpose
To enforce strict code formatting and modern C# feature usage for readability.

## Priority
Medium

## Instructions
-   **Use Expression-Bodied Members**: Prefer expression-bodied members for methods, properties, and constructors when they can be written on a single line.
-   **Declare `using` statements inside namespace**: Place `using` directives inside the `namespace` declaration to avoid conflicts and improve clarity.
-   **Use `var` sparingly**: Only use `var` when the type is obvious from the right-hand side of the assignment. Be explicit with complex types for readability.
-   **Avoid Nested Code**: Limit nested code blocks and if statements to no more than two levels. Use guard clauses to exit early instead of deeply nested logic.
-   **Simplify LINQ**: Prefer chained LINQ methods (`.Where().Select()`) over query syntax for simple transformations.
