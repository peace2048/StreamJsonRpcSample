using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;

namespace StreamJsonRpcSample
{
    class SampleListener : NamedPipeRpcListener<ISampleService>, ISampleService
    {
        public SampleListener(ILogger<SampleListener> logger, string pipeName, int maxConnection) : base(logger, pipeName, maxConnection)
        {
        }

        public Task<int> AddAsync(int a, int b)
        {
            _logger.LogInformation("AddAsync called.");
            return Task.FromResult(a + b);
        }

        protected override ISampleService OnConnect(JsonRpc rpc)
        {
            return this;
        }
    }
}