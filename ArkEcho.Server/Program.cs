using System;

namespace ArkEcho.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (Server.Instance)
            {
                if (!Server.Instance.Init())
                {
                    Console.WriteLine("Problem on Initializing the ArkEcho.Server!");
                    return;
                }
                else
                    Server.Instance.Start(); // Starts Event Cycle and API 

                if (Server.Instance.RestartRequested)
                    System.Diagnostics.Process.Start("ArkEcho.Server.exe");
            }

            Environment.Exit(0);
        }
    }
}
