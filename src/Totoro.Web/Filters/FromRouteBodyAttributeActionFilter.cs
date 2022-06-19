using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Totoro.Web.Attributes;

namespace Totoro.Web.Filters;

public class FromRouteBodyAttributeActionFilter : IActionFilter
{
    private static readonly Type AttributeType = typeof(FromRouteBodyAttribute);

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (GetActionArgument(context) is not { } argument)
        {
            return;
        }

        foreach (var property in argument.GetType().GetProperties())
        {
            if (context.RouteData.Values.TryGetValue(property.Name, out var data))
            {
                SetActionArgumentPropertyValue(property, argument, data);
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Intentionally left empty.
    }

    #region Private Methods

    private object? GetActionArgument(ActionExecutingContext context)
    {
        var parameter = context.ActionDescriptor.Parameters
            .Select(parameter => parameter as ControllerParameterDescriptor)
            .Select(parameter => parameter?.ParameterInfo)
            .FirstOrDefault(parameter => parameter?.CustomAttributes
                .Any(attribute => attribute.AttributeType == AttributeType) ?? false);

        if (string.IsNullOrWhiteSpace(parameter?.Name))
        {
            return null;
        }

        return context.ActionArguments.TryGetValue(parameter.Name, out var argument)
            ? argument
            : null;
    }

    private void SetActionArgumentPropertyValue(PropertyInfo property, object? argument, object? data)
    {
        if (data is not string text || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var converter = TypeDescriptor.GetConverter(property.PropertyType);
        var value = converter.ConvertFromInvariantString(text);

        property?.SetValue(argument, value, null);
    }

    #endregion
}
