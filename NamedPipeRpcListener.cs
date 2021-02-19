using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;

namespace StreamJsonRpcSample
{
    abstract class NamedPipeRpcListener<T>
    {
        protected readonly ILogger _logger;
        private readonly string _pipeName;
        private int _maxConnection;

        public NamedPipeRpcListener(ILogger logger, string pipeName, int maxConnection)
        {
            _logger = logger;
            _pipeName = pipeName;
            _maxConnection = maxConnection;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            using var semaphore = new SemaphoreSlim(_maxConnection, _maxConnection);
            while (!cancellationToken.IsCancellationRequested)
            {

                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    _logger.LogInformation("create pipe.");
                    var stream = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, _maxConnection, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                    _logger.LogInformation("wait for connection.");
                    await stream.WaitForConnectionAsync(cancellationToken);

                    _logger.LogInformation("create target.");
                    var rpc = new JsonRpc(stream);
                    try
                    {
                        var target = OnConnect(rpc);
                        rpc.AddLocalRpcTarget<T>(target, null);
                        rpc.StartListening();
                        rpc.Disconnected += (o, e) =>
                        {
                            _logger.LogInformation("disconnected. {reason} - {description}", e.Reason, e.Description);
                            semaphore.Release();
                        };
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "create target fail.");
                        semaphore.Release();
                    }

                }
                catch (Exception e)
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        _logger.LogError(e, "create connection fail.");
                    }
                    semaphore.Release();
                }
            }
        }

        protected abstract T OnConnect(JsonRpc rpc);
    }
}