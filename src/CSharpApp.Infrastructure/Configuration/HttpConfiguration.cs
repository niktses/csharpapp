using Microsoft.Extensions.Options;
using Polly;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var httpClientSettings = serviceProvider.GetRequiredService<IOptions<HttpClientSettings>>().Value;
        var restApiSettings = serviceProvider.GetRequiredService<IOptions<RestApiSettings>>().Value;

        services.AddHttpClient<IProductsService, ProductsService>(client =>
        {
            client.BaseAddress = new Uri(restApiSettings.BaseUrl!);
        })
        .SetHandlerLifetime(TimeSpan.FromMinutes(httpClientSettings.LifeTime))
        .AddTransientHttpErrorPolicy(policy =>
            policy.WaitAndRetryAsync(
                httpClientSettings.RetryCount,
                retryAttempt => TimeSpan.FromMilliseconds(httpClientSettings.SleepDuration * Math.Pow(2, retryAttempt - 1))
            )
        );

        return services;
    }
}