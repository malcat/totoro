using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Totoro.Web.Extensions;

public static class PollyHttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder builder, int count = 3)
    {
        return builder.AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(count, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
    }

    public static IHttpClientBuilder AddCircuitBreaker(this IHttpClientBuilder builder, int count = 5, int seconds = 30)
    {
        return builder.AddTransientHttpErrorPolicy(policy =>
            policy.CircuitBreakerAsync(count, TimeSpan.FromSeconds(seconds)));
    }
}
