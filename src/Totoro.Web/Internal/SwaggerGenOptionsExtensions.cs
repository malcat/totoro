using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Totoro.Web.Internal.Swagger;

namespace Totoro.Web.Internal;

internal static class SwaggerGenOptionsExtensions
{
    public static SwaggerGenOptions LowercaseDocuments(this SwaggerGenOptions options)
    {
        options.DocumentFilter<LowerCaseDocumentFilter>();

        return options;
    }

    public static SwaggerGenOptions AspNetCoreVersioning(this SwaggerGenOptions options)
    {
        // SchemaId already used for different type.
        options.CustomSchemaIds(type => type.ToString());

        // Replace "v{version:apiVersion}" to the actual version of the corresponding Swagger document.
        options.DocumentFilter<VersionDocumentFilter>();

        // Remove the parameter version, without it we will have the version as parameter for all endpoints in the Swagger UI.
        options.OperationFilter<VersionOperationFilter>();

        // Used to exclude endpoint mapped to not specified in Swagger version.
        options.DocInclusionPredicate(GetDocInclusionPredicate);

        // Avoid Swagger generation error due to same method name in different versions.
        options.ResolveConflictingActions(descriptions => descriptions.First());

        return options;
    }

    public static SwaggerGenOptions AddSecurity(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new()
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "Authorization",
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description = "Enter 'Bearer' [space] and your valid token."
        });

        options.AddSecurityRequirement(new()
        {
            {
                new() { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                Array.Empty<string>()
            }
        });

        return options;
    }

    #region Private Methods

    private static bool GetDocInclusionPredicate(string version, ApiDescription description)
    {
        if (!description.TryGetMethodInfo(out var method) || method.DeclaringType == null)
        {
            return false;
        }

        return method.DeclaringType
            .GetCustomAttributes(true)
            .OfType<ApiVersionAttribute>()
            .SelectMany(attribute => attribute.Versions)
            .Any(value => $"v{value}".Equals(version, StringComparison.OrdinalIgnoreCase));
    }

    #endregion
}
