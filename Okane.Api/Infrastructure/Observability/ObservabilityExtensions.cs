using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Okane.User.Application;

namespace Okane.Api.Infrastructure.Observability;

public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddOkaneLogging(this WebApplicationBuilder builder)
    {
        var resourceBuilder = builder.BuildOkaneResource();

        builder.Logging.ClearProviders();

        builder.Logging.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder);
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;
        });

        return builder;
    }

    public static WebApplicationBuilder AddOkaneTracing(this WebApplicationBuilder builder)
    {
        var resourceBuilder = builder.BuildOkaneResource();

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddSource("Npgsql")
                .AddSource(UserObservability.ActivitySourceName));

        return builder;
    }

    public static WebApplicationBuilder AddOkaneMetrics(this WebApplicationBuilder builder)
    {
        var resourceBuilder = builder.BuildOkaneResource();
        var otlpEndpoint = builder.Configuration["Observability:OtlpEndpoint"] ?? "http://localhost:4317";

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(resourceBuilder)
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint)));

        return builder;
    }

    private static ResourceBuilder BuildOkaneResource(this WebApplicationBuilder builder)
    {
        var serviceName = builder.Configuration["Observability:ServiceName"] ?? builder.Environment.ApplicationName;
        var serviceVersion = typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString() ?? "unknown";

        return ResourceBuilder.CreateDefault()
            .AddService(serviceName, serviceVersion: serviceVersion)
            .AddAttributes([new KeyValuePair<string, object>("deployment.environment", builder.Environment.EnvironmentName)]);
    }
}
