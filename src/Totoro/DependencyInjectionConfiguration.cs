using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Totoro.Extensions;
using Totoro.Options;

namespace Totoro;

public static class DependencyInjectionConfiguration
{
    #region For: IServiceCollection

    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        //
        // Libraries

        // DI from "AutoMapper".
        services.AddAutoMapper(Dependencies.Assemblies);

        // DI from "FluentValidation.AspNetCore".
        services.AddFluentValidation().AddValidatorsFromAssemblies(Dependencies.Assemblies);

        return services;
    }

    /// <summary>
    /// Add the default Core Data Protection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCoreDataProtection(this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.Load<ProtectionOptions>();

        services.AddDataProtection()
            .SetApplicationName(options.Name)
            .PersistKeysToFileSystem(new DirectoryInfo(options.Path));

        return services;
    }

    #endregion
}
