using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamJsonRpcSample
{
    interface ISampleService
    {
        Task<int> AddAsync(int a, int b);
    }
}
