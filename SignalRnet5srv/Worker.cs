using HidSharp;
using HidSharp.Reports;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppE
{
    public class Worker : BackgroundService
    {

        public static HidDevice yscanner;

        // Read only instance of IHubContext
        private readonly IHubContext<MyHub> _hub;

        // Inject IHubContext to constructor
        public Worker(IHubContext<MyHub> hub)
        {
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {


            var list = DeviceList.Local;

            list.Changed += (sender, e) => Console.WriteLine("Device list changed.");

            var hidDeviceList = list.GetHidDevices();
            foreach (HidDevice dev in hidDeviceList)
            {
                if (dev.DevicePath == @"\\?\hid#vid_046e&pid_52c3&col02#8&14b012ba&3&0001#{4d1e55b2-f16f-11cf-88cb-001111000030}")
                {
                    yscanner = dev;
                }
            }
            if (yscanner != null)
            {
                yscanner.Open();
                var reportDescriptor = yscanner.GetReportDescriptor();

                var deviceItem = reportDescriptor.DeviceItems[0];

                HidStream hidStream;
                if (yscanner.TryOpen(out hidStream))
                {

                    using (hidStream)
                    {
                        var inputReportBuffer = new byte[yscanner.GetMaxInputReportLength()];
                        var inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
                        var inputParser = deviceItem.CreateDeviceItemInputParser();
                        inputReceiver.Start(hidStream);

                        int startTime = Environment.TickCount;
                        while (!stoppingToken.IsCancellationRequested)
                        {
                            if (inputReceiver.WaitHandle.WaitOne(1000))
                            {
                                if (!inputReceiver.IsRunning) { break; } // Disconnected?

                                Report report;
                                while (inputReceiver.TryRead(inputReportBuffer, 0, out report))
                                {
                                    // Parse the report if possible.
                                    // This will return false if (for example) the report applies to a different DeviceItem.
                                    if (inputParser.TryParseReport(inputReportBuffer, 0, report))
                                    {
                                        await _hub.Clients.All.SendAsync("Broadcast", Encoding.ASCII.GetString(inputReportBuffer));
                                        Console.WriteLine();
                                        //WriteDeviceItemInputParserResult(inputParser);
                                    }
                                }
                            }
                            await Task.Delay(100);
                        }
                    }
                }
            }


            /*
            while (!stoppingToken.IsCancellationRequested)
            {
                // Broadcast current time to all clients
                await _hub.Clients.All.SendAsync("Broadcast", DateTime.Now.ToString());
                // Put sleep for 10 seconds
                await Task.Delay(500);
            }
            */
        }
    }
}
