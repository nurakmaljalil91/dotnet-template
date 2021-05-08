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
        private readonly global::SocketServerLib.SocketServerLib _server;
        public bool IsRunning { get; set; }


        private int? _exitCode; // check if application exit

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="log"></param>
        /// <param name="appLifetime"></param>
        public Application(
            ILogger<Application> log,
            IHostApplicationLifetime appLifetime)
        {
            _log = log;
            _appLifetime = appLifetime;
            _server = new global::SocketServerLib.SocketServerLib();
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

                        // Ready to start tcp server here
                        _log.LogInformation("Starting the TCP Server");

                        IsRunning = true;

                        _server.Listen();
                        
                        _log.LogInformation("End of server");
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