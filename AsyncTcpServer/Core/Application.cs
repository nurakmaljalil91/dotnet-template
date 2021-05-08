using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AsyncTcpServer.Core
{
    internal sealed class Application : IHostedService
    {
        private readonly ILogger<Application> _log;
        private readonly IHostApplicationLifetime _appLifetime;


        private int? _exitCode; // check if application exit

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="log"></param>
        /// <param name="appLifetime"></param>
        public Application(
            ILogger<Application> log,
            IHostApplicationLifetime appLifetime
            )
        {
            _log = log;
            _appLifetime = appLifetime;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _log.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _log.LogInformation("Hello World!");

                        // Simulate real work is being done
                        for (var i = 0; i < 1000; i++)
                        {
                            Console.WriteLine(i);
                        }

                        await Task.Delay(1000, cancellationToken);

                        _exitCode = 0;
                    }
                    catch (Exception ex)
                    {
                        _log.LogError("Unhandled exception! {0}", ex);

                        _exitCode = 1;
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                }, cancellationToken);
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _log.LogDebug("Exiting with return code: {0}", _exitCode);

            return Task.CompletedTask;
        }
    }
}