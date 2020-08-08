using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArkEcho.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            bool restart = false;

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

                host.Run();

                restart = ArkEchoServer.Instance.RestartRequested;
            }

            if (restart)
                System.Diagnostics.Process.Start("ArkEcho.Server.exe");

            Environment.Exit(0);
        }
    }
}
