using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamJsonRpcSample
{
    public class NamedPipeSingleRpcServer<TInterface>
    {
        private readonly ILogger _logger;
        private readonly string _pipeName;
        private readonly Func<TInterface> _create;
        private CancellationTokenSource _cancellation;
        private Task _listenTask;


        public NamedPipeSingleRpcServer(ILogger<NamedPipeSingleRpcServer<TInterface>> logger, string pipeName, Func<TInterface> create)
        {
            _logger = logger;
            _pipeName = pipeName;
            _create = create ?? throw new ArgumentNullException(nameof(create));
        }

        public void Start()
        {
            _cancellation = new CancellationTokenSource();
            _listenTask = StartAsync();
        }

        public async Task StopAsync()
        {
            _cancellation.Cancel();
            await _listenTask.ContinueWith(_ => { }, TaskScheduler.Default);
        }

        private async Task StartAsync()
        {
            while (!_cancellation.Token.IsCancellationRequested)
            {
                using (var pipe = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
                {
                    try
                    {
                        await pipe.WaitForConnectionAsync(_cancellation.Token);
                        using (var rpc = new JsonRpc(pipe))
                        {
                            var target = _create();
                            rpc.AddLocalRpcTarget<TInterface>(target, null);
                            rpc.StartListening();
                            await rpc.Completion;
                        }
                    }
                    catch (Exception e)
                    {
                        if (!_cancellation.Token.IsCancellationRequested)
                        {
                            _logger.LogError(e, "");
                        }
                    }
                }
            }
        }
    }
}
