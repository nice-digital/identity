using Microsoft.Extensions.Configuration;
using NICE.Logging;
using NICE.Logging.Sinks.RabbitMQ;
using Serilog;
using System;

namespace NICE.Identity.Authorisation.WebAPI
{
    /// <summary>
    /// This has been refactored for .NET Core 3.1 
    /// 
    /// It now just sets up a LoggerConfiguration object based off appsettings.json (and secrets.json on dev machines)
    /// </summary>
    public static class SeriLogger 
    {
        public static LoggerConfiguration GetLoggerConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddUserSecrets<Startup>()
                .Build();

            // read appsettings
            var logCfg = config.GetSection("Logging");
                
            var rabbitSettingsFound = int.TryParse(logCfg["RabbitMQPort"], out var rPort);
            bool.TryParse(logCfg["UseRabbit"], out var useRabbit);
            string logFilePath = logCfg["LogFilePath"]; ;

            var formatter = new NiceSerilogFormatter(logCfg["Environment"], "IdentityApi");
            var logConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .MinimumLevel
                .Warning();

            if (rabbitSettingsFound && useRabbit)
            {
                var rHost = logCfg["RabbitMQHost"];
             
                var rabbit = new RabbitMQConfiguration {
                    Hostname = rHost,
                    Port = rPort,
                    Protocol = RabbitMQ.Client.Protocols.AMQP_0_9_1,
                    Exchange = "logging.application.serilog",
                    ExchangeType = "topic"
                };

                logConfig.WriteTo.RabbitMQ(rabbit, formatter);
            }

            bool.TryParse(logCfg["UseFile"], out var useFile);

            if (useFile) //probably dev only
                logConfig.WriteTo.RollingFile(formatter, logFilePath, fileSizeLimitBytes: 5000000, retainedFileCountLimit: 5, flushToDiskInterval: TimeSpan.FromSeconds(20));

            return logConfig;
        }
    }
}
