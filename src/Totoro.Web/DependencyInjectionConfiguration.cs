using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Totoro.Extensions;
using Totoro.Web.Factories;
using Totoro.Web.Filters;
using Totoro.Web.Internal;
using Totoro.Web.Middlewares;
using Totoro.Web.Options;

namespace Totoro.Web;

public static class DependencyInjectionConfiguration
{
    #region For: IServiceCollection

    public static IServiceCollection AddCoreWeb(this IServiceCollection services)
    {
        //
        // Libraries

        // DI from "Microsoft.AspNetCore.Mvc.Versioning".
        services.AddApiVersioning();

        //
        // Factories

        services
            .AddTransient<IClientErrorFactory, ClientErrorFactory>();

        return services;
    }

    public static IServiceCollection AddCoreWebControllers(this IServiceCollection services, Action<MvcOptions> setup = null!)
    {
        var builder = services.AddControllers(options =>
        {
            // Do not change the order.
            options.Filters.Add<FromRouteBodyAttributeActionFilter>();
            options.Filters.Add<FailFastActionFilter>();

            setup?.Invoke(options);
        });

        builder.AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        builder.ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });

        return services;
    }

    public static IServiceCollection AddCoreWebSwagger(this IServiceCollection services, Action<SwaggerGenOptions> setup = null!)
    {
        services.AddSwaggerGen(options =>
        {
            options.LowercaseDocuments();

            options.AspNetCoreVersioning();

            options.AddSecurity();

            setup?.Invoke(options);
        });

        return services;
    }

    #endregion

    #region For: IApplicationBuilder

    public static IApplicationBuilder UseCoreWebMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        return app;
    }

    public static IApplicationBuilder UseCoreWebRequestLocalization(this IApplicationBuilder app)
    {
        var configuration = app.ApplicationServices.GetService<IConfiguration>();

        if (configuration == null)
        {
            throw new InvalidOperationException();
        }

        var options = configuration.Load<CultureOptions>();
        var cultures = options.Supported?.Select(name => new CultureInfo(name)).ToArray();

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(options.Default),
            SupportedCultures = cultures,
            SupportedUICultures = cultures
        });

        return app;
    }

    public static IApplicationBuilder UseCoreWebSwagger(this IApplicationBuilder app, Action<SwaggerUIOptions> setup = null!)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = string.Empty;

            options.DefaultModelsExpandDepth(-1);

            setup?.Invoke(options);
        });

        return app;
    }

    public static IApplicationBuilder UseCoreWebEndpoints(this IApplicationBuilder app, Action<IEndpointRouteBuilder> setup = null!)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            setup?.Invoke(endpoints);
        });

        return app;
    }

    #endregion
}
