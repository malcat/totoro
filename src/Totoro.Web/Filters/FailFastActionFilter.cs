using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Totoro.Web.Filters;

public class FailFastActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid)
        {
            return;
        }

        var validations = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage).ToArray();

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Result = new ObjectResult(Result.Fail(Errors.Validation, validations));
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Intentionally left empty.
    }
}
