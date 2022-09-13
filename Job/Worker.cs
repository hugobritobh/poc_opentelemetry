using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Job
{
    public sealed class Worker : BackgroundService
    {
        public static readonly string NAME = "Test-Job";

        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger,
                      IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await TraceAsync(async (ct) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2), ct);

                _logger.LogInformation("Process...");

                await Task.Delay(TimeSpan.FromSeconds(5), ct);

                throw new ApplicationException("App Exception !!!");

            }, stoppingToken).ConfigureAwait(false);
        }

        private async ValueTask TraceAsync(Func<CancellationToken, ValueTask> method, CancellationToken stoppingToken)
        {
            var providerTracer = _serviceProvider.GetService<TracerProvider>();
            var trace = providerTracer?.GetTracer(Worker.NAME);
            var name = this.GetType().Name;
            using var span = trace?.StartRootSpan(name);
            try
            {
                span?.AddEvent($"Start {name}");

                _logger.LogInformation("Start Job");

                await method(stoppingToken).ConfigureAwait(false);

                _logger.LogInformation("End Job: Success");
            }
            catch (Exception ex)
            {
                span?.RecordException(ex);
                _logger.LogCritical("End Job: Error", ex);
            }
            finally
            {
                span?.AddEvent($"End {name}");
                span?.End();
            }
        }
    }
}