using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace JSONAdminEditor.API;

public class OpenApiDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "JSON Admin Editor API",
            Version = "v1",
            Description = "Modern API for managing notification configurations and reference data",
            Contact = new OpenApiContact
            {
                Name = "Development Team"
            }
        };

        // Add server information
        document.Servers =
        [
            new() { Url = "/", Description = "Current server" }
        ];

        // Enhance schema generation for better client generation
        if (document.Components?.Schemas != null)
        {
            foreach (var schema in document.Components.Schemas.Values)
            {
                if (schema.Properties != null)
                {
                    foreach (var property in schema.Properties.Values)
                    {
                        if (string.IsNullOrEmpty(property.Description))
                        {
                            property.Description = "Property description";
                        }
                    }
                }
            }
        }

        return Task.CompletedTask;
    }
}