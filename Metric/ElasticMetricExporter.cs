//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using OpenTelemetry;
//using OpenTelemetry.Resources;
//using OpenTelemetry.Trace;
//using System.Diagnostics;
//using System.Runtime.CompilerServices;

//namespace Metric
//{

//    public static class MetricsExtentions
//    {
//        public static TracerProviderBuilder AddElasticLoggerExporter(this TracerProviderBuilder builder, IServiceCollection services)
//        {
//            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
//            {
//                var log = serviceProvider.GetService<ILogger<ElasticMetricExporter>>();
//                return builder.AddProcessor(new BatchActivityExportProcessor(new ElasticMetricExporter(log)));
//            }
//        }
//    }

//    public partial class ElasticMetricExporter : BaseExporter<Activity>
//    {
//        private readonly ILogger<ElasticMetricExporter> _logger;

//        public ElasticMetricExporter(ILogger<ElasticMetricExporter> log)
//        {
//            _logger = log;
//        }

//        [LoggerMessage(Level = LogLevel.Information, Message = "Activity.TraceId: {traceId}, Activity.SpanId: {spanId}, Activity.ParentId: {parentId}, Activity.OperationName: {operationName}, Activity.TraceFlags:  {activityTraceFlags}, Activity.TraceState: {traceStateString}, Activity.ParentSpanId: {parentSpanId}, Activity.ActivitySourceName: {sourceName}, Activity.DisplayName: {displayName}, Activity.Kind: {kind}, Activity.StartTimeUtc: {startTimeUtc}, Activity.Duration: {duration}, Http.host: {httpHost}, Http.Method: {httpMethod}, Http.Scheme: {httpScheme}, Http.Target: {httpTarget}, Http.Url: {httpUrl}, Http.StatusCode: {httpStatusCode}, Activity.AssociatedId: {associatedId}", SkipEnabledCheck = true, EventId = 11)]
//        partial void MetricHttp(string traceId, string spanId, string? parentId, string operationName, string activityTraceFlags, string? traceStateString, string parentSpanId, string sourceName, string displayName, string kind, DateTime startTimeUtc, TimeSpan duration, string? httpHost, string? httpMethod, string? httpScheme, string? httpTarget, string? httpUrl, string? httpStatusCode, string? associatedId);

//        [LoggerMessage(Level = LogLevel.Information, Message = "Activity.TraceId: {traceId}, Activity.SpanId: {spanId}, Activity.ParentId: {parentId}, Activity.OperationName: {operationName}, Activity.TraceFlags:  {activityTraceFlags}, Activity.TraceState: {traceStateString}, Activity.ParentSpanId: {parentSpanId}, Activity.ActivitySourceName: {sourceName}, Activity.DisplayName: {displayName}, Activity.Kind: {kind}, Activity.StartTimeUtc: {startTimeUtc}, Activity.Duration: {duration}, Tags: {tags}, Activity.AssociatedId: {associatedId}", SkipEnabledCheck = true, EventId = 12)]
//        partial void Metric(string traceId, string spanId, string? parentId, string operationName, string activityTraceFlags, string? traceStateString, string parentSpanId, string sourceName, string displayName, string kind, DateTime startTimeUtc, TimeSpan duration, string tags, string? associatedId);
//    }

//    public partial class ElasticMetricExporter : BaseExporter<Activity>
//    {
//        public override ExportResult Export(in Batch<Activity> batch)
//        {
//            using var scope = SuppressInstrumentationScope.Begin();

//            foreach (var activity in batch)
//            {
//                string? associatedId = string.Empty, httpHost = string.Empty, httpMethod = string.Empty, httpScheme = string.Empty, httpTarget = string.Empty, httpUrl = string.Empty, httpStatusCode = string.Empty;

//                var resource = this.ParentProvider.GetResource();
//                if (resource != Resource.Empty)
//                {
//                    associatedId = Unsafe.As<string>(resource.Attributes.FirstOrDefault(c => c.Key == "service.instance.id").Value);
//                }

//                if (activity.Source.Name.Contains("Http") || activity.Source.Name.Contains("AspNet"))
//                {

//                    if (activity.TagObjects != null)
//                    {
//                        foreach (var item in activity.TagObjects)
//                        {
//                            if (item.Key == "http.host")
//                            {
//                                httpHost = Unsafe.As<string>(item.Value);
//                            }
//                            else if (item.Key == "http.method")
//                            {
//                                httpMethod = Unsafe.As<string>(item.Value);
//                            }
//                            else if (item.Key == "http.scheme")
//                            {
//                                httpScheme = Unsafe.As<string>(item.Value);
//                            }
//                            else if (item.Key == "http.target")
//                            {
//                                httpTarget = Unsafe.As<string>(item.Value);
//                            }
//                            else if (item.Key == "http.url")
//                            {
//                                httpUrl = Unsafe.As<string>(item.Value);
//                            }
//                            else if (item.Key == "http.status_code")
//                            {
//                                httpStatusCode = item.Value?.ToString();
//                            }
//                        }
//                    }

//                    if (activity.Status != ActivityStatusCode.Unset)
//                    {
//                        httpStatusCode = activity.Status.ToString();
//                    }

//                    MetricHttp(activity.TraceId.ToString(),
//                                   activity.SpanId.ToString(),
//                                   activity.ParentId,
//                                   activity.OperationName,
//                                   activity.ActivityTraceFlags.ToString(),
//                                   activity.TraceStateString,
//                                   activity.ParentSpanId.ToString(),
//                                   activity.Source.Name,
//                                   activity.DisplayName,
//                                   activity.Kind.ToString(),
//                                   activity.StartTimeUtc,
//                                   activity.Duration,
//                                   httpHost,
//                                   httpMethod,
//                                   httpScheme,
//                                   httpTarget,
//                                   httpUrl,
//                                   httpStatusCode,
//                                   associatedId);
//                }
//                else
//                {
//                    string tags = string.Empty;

//                    if (activity.TagObjects != null)
//                    {
//                        try
//                        {
//                            tags = System.Text.Json.JsonSerializer.Serialize(activity.TagObjects);
//                        }
//                        catch (Exception) { }
//                    }

//                    Metric(activity.TraceId.ToString(),
//                           activity.SpanId.ToString(),
//                           activity.ParentId,
//                           activity.OperationName,
//                           activity.ActivityTraceFlags.ToString(),
//                           activity.TraceStateString,
//                           activity.ParentSpanId.ToString(),
//                           activity.Source.Name,
//                           activity.DisplayName,
//                           activity.Kind.ToString(),
//                           activity.StartTimeUtc,
//                           activity.Duration,
//                           tags,
//                           associatedId);
//                }
//            }

//            return ExportResult.Success;

//            //foreach (var activity in batch)
//            //{
//            //    this.WriteLine($"Activity.TraceId: {activity.TraceId}");
//            //    this.WriteLine($"Activity.SpanId: {activity.SpanId}");
//            //    this.WriteLine($"Activity.TraceFlags:  {activity.ActivityTraceFlags}");
//            //    if (!string.IsNullOrEmpty(activity.TraceStateString))
//            //    {
//            //        this.WriteLine($"Activity.TraceState:    {activity.TraceStateString}");
//            //    }

//            //    if (activity.ParentSpanId != default)
//            //    {
//            //        this.WriteLine($"Activity.ParentSpanId:    {activity.ParentSpanId}");
//            //    }

//            //    this.WriteLine($"Activity.ActivitySourceName: {activity.Source.Name}");
//            //    this.WriteLine($"Activity.DisplayName: {activity.DisplayName}");
//            //    this.WriteLine($"Activity.Kind:        {activity.Kind}");
//            //    this.WriteLine($"Activity.StartTime:   {activity.StartTimeUtc:yyyy-MM-ddTHH:mm:ss.fffffffZ}");
//            //    this.WriteLine($"Activity.Duration:    {activity.Duration}");
//            //    var statusCode = string.Empty;
//            //    var statusDesc = string.Empty;

//            //    if (activity.TagObjects.Any())
//            //    {
//            //        this.WriteLine("Activity.Tags:");
//            //        foreach (var tag in activity.TagObjects)
//            //        {
//            //            if (tag.Key == SpanAttributeConstants.StatusCodeKey)
//            //            {
//            //                statusCode = tag.Value as string;
//            //                continue;
//            //            }

//            //            if (tag.Key == SpanAttributeConstants.StatusDescriptionKey)
//            //            {
//            //                statusDesc = tag.Value as string;
//            //                continue;
//            //            }

//            //            if (ConsoleTagTransformer.Instance.TryTransformTag(tag, out var result))
//            //            {
//            //                this.WriteLine($"    {result}");
//            //            }
//            //        }
//            //    }

//            //    if (activity.Status != ActivityStatusCode.Unset)
//            //    {
//            //        this.WriteLine($"StatusCode : {activity.Status}");
//            //        if (!string.IsNullOrEmpty(activity.StatusDescription))
//            //        {
//            //            this.WriteLine($"Activity.StatusDescription : {activity.StatusDescription}");
//            //        }
//            //    }
//            //    else if (!string.IsNullOrEmpty(statusCode))
//            //    {
//            //        this.WriteLine($"   StatusCode : {statusCode}");
//            //        if (!string.IsNullOrEmpty(statusDesc))
//            //        {
//            //            this.WriteLine($"   Activity.StatusDescription : {statusDesc}");
//            //        }
//            //    }

//            //    if (activity.Events.Any())
//            //    {
//            //        this.WriteLine("Activity.Events:");
//            //        foreach (var activityEvent in activity.Events)
//            //        {
//            //            this.WriteLine($"    {activityEvent.Name} [{activityEvent.Timestamp}]");
//            //            foreach (var attribute in activityEvent.Tags)
//            //            {
//            //                if (ConsoleTagTransformer.Instance.TryTransformTag(attribute, out var result))
//            //                {
//            //                    this.WriteLine($"        {result}");
//            //                }
//            //            }
//            //        }
//            //    }

//            //    if (activity.Links.Any())
//            //    {
//            //        this.WriteLine("Activity.Links:");
//            //        foreach (var activityLink in activity.Links)
//            //        {
//            //            this.WriteLine($"    {activityLink.Context.TraceId} {activityLink.Context.SpanId}");
//            //            foreach (var attribute in activityLink.Tags)
//            //            {
//            //                if (ConsoleTagTransformer.Instance.TryTransformTag(attribute, out var result))
//            //                {
//            //                    this.WriteLine($"        {result}");
//            //                }
//            //            }
//            //        }
//            //    }

//            //    var resource = this.ParentProvider.GetResource();
//            //    if (resource != Resource.Empty)
//            //    {
//            //        this.WriteLine("Resource associated with Activity:");
//            //        foreach (var resourceAttribute in resource.Attributes)
//            //        {
//            //            if (ConsoleTagTransformer.Instance.TryTransformTag(resourceAttribute, out var result))
//            //            {
//            //                this.WriteLine($"    {result}");
//            //            }
//            //        }
//            //    }

//            //    this.WriteLine(string.Empty);
//            //}

//        }
//    }
//}