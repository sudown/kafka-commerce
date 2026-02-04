using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace SistemaPedidos.API.Config.Extensions
{
    public static class SerilogExtensions
    {
        public static void AddStandardLogging(this IHostBuilder builder)
        {
            builder.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", context.HostingEnvironment.ApplicationName)
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = "http://localhost:4317";
                        options.Protocol = OtlpProtocol.Grpc;
                    });
            });
        }
    }
}
