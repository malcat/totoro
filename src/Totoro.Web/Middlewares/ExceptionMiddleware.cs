using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Totoro.Web.Extensions;

namespace Totoro.Web.Middlewares;

public class ExceptionMiddleware
{
    private const string NoCache = "no-cache, no-store, must-revalidate";

    private readonly ILogger<ExceptionMiddleware> _logger;

    private readonly RequestDelegate _next;

    public ExceptionMiddleware(
        ILogger<ExceptionMiddleware> logger,
        RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    #region Private Methods

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Exception Handler Middleware ({0})", context.TraceIdentifier);

        context.Response.Headers.Add(HeaderNames.CacheControl, NoCache);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var result = Result.Fail(Errors.Woops, null!, new
        {
            Trace = context.TraceIdentifier
        });

        await context.Response.WriteAsync(result.ToJson());
    }

    #endregion
}
