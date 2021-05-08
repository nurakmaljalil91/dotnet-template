using System;
using System.IO;
using System.Threading.Tasks;
using AsyncTcpServer.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace AsyncTcpServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();

            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    "Logs\\log.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:G} {Message}{NewLine:1}{Exception:1}")
                .CreateLogger();

            Log.Logger.Information("Program starting");

            try
            {
                await Host.CreateDefaultBuilder(args)
                    .ConfigureServices((context, services) =>
                    {
                        services
                            .AddSingleton<IHostedService, Application>();

                    })
                    .UseSerilog()
                    .RunConsoleAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Program failed to start correctly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static void BuildConfig(IConfigurationBuilder builder)
        {
            // builder go to current directory, find json and reload changes
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.json.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    optional: true) // Check development environment
                .AddEnvironmentVariables();

            // NOTES: set appsettings.json copy as to output directory as copy always
        }
    }
}
