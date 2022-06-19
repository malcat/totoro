using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Totoro.Extensions;
using Totoro.Transport.Attributes;
using Totoro.Transport.Behaviors;
using Totoro.Transport.Filters;
using Totoro.Transport.Internal;
using Totoro.Transport.Options;

namespace Totoro.Transport;

public static class DependencyInjectionConfiguration
{
    #region For: IServiceCollection

    public static IServiceCollection AddCoreTransport(this IServiceCollection services, IConfiguration configuration)
    {
        //
        // Libraries

        // DI from "MediatR".
        AddMediatR(services);

        // DI from "MassTransit".
        AddMassTransit(services, configuration);

        //
        // Services

        services.AddTransient<IBus, Bus>();

        return services;
    }

    #endregion

    #region Private Methods

    private static IEnumerable<Type> GetRemoteConsumers()
    {
        var target = typeof(Handler<,>);

        return Dependencies.Types
            .Where(type => type.IsSubclassOfGeneric(target))
            .Where(type => type.GetCustomAttributes(true).OfType<RemoteAttribute>().Any());
    }

    private static void AddMediatR(IServiceCollection services)
    {
        // Local Consumers.
        Dependencies.Assemblies.ToList().ForEach(assembly => services.AddMediatR(assembly));

        // Behaviors.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FailFastRequestBehavior<,>));
    }

    private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.Load<AmazonSqsOptions>();
        var consumers = GetRemoteConsumers().ToList();

        // Services overrides.
        services.AddSingleton<IEndpointNameFormatter>(new CustomEndpointNameFormatter(options.Prefix));

        services.AddMassTransit((configurator) =>
        {
            // Endpoint name format.
            configurator.SetKebabCaseEndpointNameFormatter();

            // Remote Consumers.
            consumers.ForEach(type => configurator.AddConsumer(type));

            // Transport: Remote.
            configurator.UsingAmazonSqs((context, bus) =>
            {
                bus.Host(options.Host, (host) =>
                {
                    host.AccessKey(options.AccessKey);
                    host.SecretKey(options.SecretKey);
                });

                // Filters.
                bus.UseConsumeFilter(typeof(ExceptionFilter<>), context);
                bus.UseConsumeFilter(typeof(FailFastRequestFilter<>), context);

                // Endpoints.
                bus.ConfigureEndpoints(context);
            });
        });
    }

    #endregion
}
