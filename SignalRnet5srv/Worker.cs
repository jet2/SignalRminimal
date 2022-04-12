using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppE
{
    public class Worker : BackgroundService
    {
        // Read only instance of IHubContext
        private readonly IHubContext<MyHub> _hub;

        // Inject IHubContext to constructor
        public Worker(IHubContext<MyHub> hub)
        {
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Broadcast current time to all clients
                await _hub.Clients.All.SendAsync("Broadcast", DateTime.Now.ToString());
                // Put sleep for 10 seconds
                await Task.Delay(500);
            }
        }
    }
}
