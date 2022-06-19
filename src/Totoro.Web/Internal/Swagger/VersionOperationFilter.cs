using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Totoro.Web.Internal.Swagger;

internal class VersionOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var version = operation.Parameters.Single(parameter =>
            parameter.Name.Equals("version", StringComparison.OrdinalIgnoreCase));

        operation.Parameters.Remove(version);
    }
}
