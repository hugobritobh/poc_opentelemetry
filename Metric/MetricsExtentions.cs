using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace Metric
{
    public static class MetricsExtentions
    {
        public static IServiceCollection Metrics(this IServiceCollection services, IConfiguration config, string serviceName)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //Signoz
            var url = new Uri("http://3.4.255.123:4317");
            var serviceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

            services.AddOpenTelemetryMetrics(b =>
            {
                b
                .AddMeter(serviceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                    .AddEnvironmentVariableDetector()
                    .AddTelemetrySdk())
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddEventCounterMetrics(o => o.RefreshIntervalSecs = 5)
#if DEBUG
                //.AddConsoleExporter()
#endif
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = url;
                    opt.Protocol = OtlpExportProtocol.Grpc;
                });
            });

            services.AddOpenTelemetryTracing(b =>
            {
                b
                .AddSource(serviceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                    .AddEnvironmentVariableDetector()
                    .AddTelemetrySdk())
                //SqlServer, have to Npgsql (Postgre)
                .AddSqlClientInstrumentation(option =>
                {
                    option.SetDbStatementForText = true;
                    option.RecordException = true;
                })
                .AddAWSInstrumentation()
                .AddAspNetCoreInstrumentation(option => { option.RecordException = true; })
                .AddHttpClientInstrumentation(option => { option.RecordException = true; })
                .SetErrorStatusOnException(true)
#if DEBUG
                .AddConsoleExporter()
#endif
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = url;
                    opt.Protocol = OtlpExportProtocol.Grpc;
                });
            });

            return services;
        }
    }
}
