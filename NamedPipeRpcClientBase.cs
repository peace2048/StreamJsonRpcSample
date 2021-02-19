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
    class NamedPipeRpcClientBase
    {
        protected readonly ILogger _logger;
        private readonly string _pipeName;
        private NamedPipeClientStream _pipeStream;
        protected JsonRpc _jsonRpc;

        public NamedPipeRpcClientBase(ILogger logger, string pipeName)
        {
            _logger = logger;
            _pipeName = pipeName;
        }

        public async Task ConnectAsync() => await ConnectAsync(CancellationToken.None);

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            _pipeStream = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            await _pipeStream.ConnectAsync(cancellationToken);
            _jsonRpc = new JsonRpc(_pipeStream);
            _jsonRpc.StartListening();
        }

        public void Disconnect()
        {
            _jsonRpc.Dispose();
            _pipeStream.Dispose();
        }
    }
}
