using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class CustomRequestBodyFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.Name == "CreateRental") // Reemplaza con el nombre de tu método
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["userId"] = new OpenApiSchema { Type = "integer" },
                                ["vehicleId"] = new OpenApiSchema { Type = "integer" },
                                ["startDate"] = new OpenApiSchema { Type = "string", Format = "date-time" },
                                ["totalDays"] = new OpenApiSchema { Type = "integer" },
                                ["isDeleted"] = new OpenApiSchema { Type = "boolean" }
                            },
                            Required = new HashSet<string>
                            {
                                "userId", "vehicleId", "startDate", "totalDays", "isDeleted"
                            }
                        }
                    }
                }
            };
        }
    }
}
