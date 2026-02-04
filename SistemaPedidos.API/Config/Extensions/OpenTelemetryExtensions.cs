using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;
using System;
using System.Diagnostics;

namespace SistemaPedidos.API.Config.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static WebApplicationBuilder AddStandardOpenTelemetry(this WebApplicationBuilder builder)
        {
            const string CUSTOM_SOURCE_NAME = "KafkaCommerce.CustomSource";

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddSource(CUSTOM_SOURCE_NAME)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddConsoleExporter();
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddConsoleExporter();
                });

            builder.Services.AddSingleton(new ActivitySource(CUSTOM_SOURCE_NAME));

            return builder;
        }
    }
}
