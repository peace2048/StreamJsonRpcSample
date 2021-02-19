using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamJsonRpcSample
{
    internal class SampleService : ISampleService
    {
        public Task<int> AddAsync(int a, int b) => Task.FromResult(a + b);
    }
}
