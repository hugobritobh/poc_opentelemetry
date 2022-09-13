using Job;
using Metric;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var configBuilder = new ConfigurationBuilder()
                          .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: false)
                          .Build();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //NOTE: OpenTelemetry com Metric\Trace mandando para Signoz
        services.Metrics(configBuilder, Worker.NAME);

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

