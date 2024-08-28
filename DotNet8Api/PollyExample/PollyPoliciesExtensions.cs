using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Net;
using System.Net.Http;

public static class PollyPoliciesExtensions
{
    public static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
    {
        IAsyncPolicy<HttpResponseMessage> retryPolicyWithLogging = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (outcome, timespan, retryCount, context) =>
                {
                    Log.Warning($"Retry {retryCount} implemented with {timespan} seconds delay due to {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                });

        IAsyncPolicy<HttpResponseMessage> timeoutPolicy = Policy
            .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));

        IAsyncPolicy<HttpResponseMessage> circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30)) as IAsyncPolicy<HttpResponseMessage>;

        IAsyncPolicy<HttpResponseMessage> bulkheadPolicy = Policy
            .BulkheadAsync<HttpResponseMessage>(maxParallelization: 10, maxQueuingActions: 5);

        IAsyncPolicy<HttpResponseMessage> fallbackPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.InternalServerError)
            .FallbackAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("This is a fallback response")
            });

        var combinedPolicy = Policy.WrapAsync(retryPolicyWithLogging, circuitBreakerPolicy, timeoutPolicy);

        return combinedPolicy;
    }
}
