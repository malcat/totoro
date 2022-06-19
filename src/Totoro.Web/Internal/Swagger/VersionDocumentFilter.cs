using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Totoro.Web.Internal.Swagger;

internal class VersionDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();

        var dictionary = document.Paths.ToDictionary(
            path => path.Key.Replace("v{version}", document.Info.Version),
            path => path.Value
        );

        foreach (var (key, value) in dictionary)
        {
            paths.Add(key, value);
        }

        document.Paths = paths;
    }
}
