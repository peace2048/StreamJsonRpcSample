using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ConsoleAppFramework;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StreamJsonRpcSample
{
    class Program : ConsoleAppBase
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(provider =>
                    {
                        var logger = provider.GetService<ILogger<NamedPipeSingleRpcServer<ISampleService>>>();
                        return new NamedPipeSingleRpcServer<ISampleService>(logger, "SampleRpc", () => new SampleService());
                    });
                    services.AddSingleton(provider =>
                    {
                        var logger = provider.GetService<ILogger<SampleListener>>();
                        return new SampleListener(logger, "SampleRpc", 2);
                    });
                    services.AddTransient(provider =>
                    {
                        var logger = provider.GetService<ILogger<SampleClient>>();
                        return new SampleClient(logger, "SampleRpc");
                    });
                })
                .RunConsoleAppFrameworkAsync<Program>(args);
        }

#pragma warning disable VSTHRD200
        [Command("Server")]
        public async Task Server()
#pragma warning restore
        {
            var server = Context.ServiceProvider.GetService<NamedPipeSingleRpcServer<ISampleService>>();
            server.Start();
            Console.WriteLine("START");
            try
            {
                await Task.Delay(System.Threading.Timeout.Infinite, Context.CancellationToken);
            }
            catch (Exception)
            {
                if (!Context.CancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                Console.WriteLine("END");
            }
            finally
            {
                await server.StopAsync();
            }
        }

#pragma warning disable VSTHRD200
        [Command("Listen")]
        public async Task Listen()
#pragma warning restore
        {
            var listener = Context.ServiceProvider.GetService<SampleListener>();
            await listener.RunAsync(Context.CancellationToken);
        }

#pragma warning disable VSTHRD200
        [Command("Client")]
        public async Task Client()
#pragma warning restore
        {
            var client = Context.ServiceProvider.GetService<SampleClient>();
            await client.ConnectAsync();
            for (var i = 0; i < 3; i++)
            {
                var r = await client.AddAsync(i, i);
                Console.WriteLine($"{i} + {i} = {r}");
                Console.ReadLine();
            }
            client.Disconnect();
        }
    }
}
