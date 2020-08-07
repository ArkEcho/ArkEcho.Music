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
                IWebHost host = WebHost.CreateDefaultBuilder(args)
                                .UseUrls("https://*:5001")
                                .UseKestrel()
                                .UseStartup<Startup>()
                                .Build();

                if (host == null)
                    return;

                // This may take a while
                Task.Factory.StartNew(() => ArkEchoServer.Instance.Init(host));

                host.Run();

                restart = ArkEchoServer.Instance.RestartRequested;

                host.Dispose();
                host = null;
            }

            if (restart)
                System.Diagnostics.Process.Start("ArkEcho.Server.exe");

            Environment.Exit(0);
        }
    }
}
