using System;

namespace ArkEcho.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (ArkEchoServer.Instance)
            {
                if (!ArkEchoServer.Instance.Init())
                {
                    Console.WriteLine("Problem on Initializing the ArkEcho.Server!");
                    return;
                }
                else
                    ArkEchoServer.Instance.Start(); // Starts Event Cycle and API 

                if (ArkEchoServer.Instance.RestartRequested)
                    System.Diagnostics.Process.Start("ArkEcho.Server.exe");
            }

            Environment.Exit(0);
        }
    }
}
