using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamJsonRpcSample
{
    class SampleClient : NamedPipeRpcClientBase, ISampleService
    {
        public SampleClient(ILogger<SampleClient> logger, string pipeName) : base(logger, pipeName)
        {
        }

        public async Task<int> AddAsync(int a, int b) => await _jsonRpc.InvokeAsync<int>(nameof(ISampleService.AddAsync), a, b);
    }
}
