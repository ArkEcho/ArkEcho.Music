using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace ArkEcho.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (ArkEchoServer.Instance)
            {
                // Start the WebHost, Server and Controllers
                using IWebHost host = WebHost.CreateDefaultBuilder(args)
                                .UseUrls("https://*:5001")
                                .UseKestrel()
                                .UseStartup<Startup>()
                                .Build();

                if (host == null)
                    return;

                if (!ArkEchoServer.Instance.Init(host))
                {
                    Console.WriteLine("Problem on Initializing the ArkEcho.Server!");
                    return;
                }
                else
                {
                    host.Run(); // Starts Event Cycle and API

                    if (ArkEchoServer.Instance.RestartRequested)
                        System.Diagnostics.Process.Start("ArkEcho.Server.exe");
                }
            }

            Environment.Exit(0);
        }
    }
}
