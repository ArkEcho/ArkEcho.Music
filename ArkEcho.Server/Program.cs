using System;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            test();
            using (Server.Instance)
            {
                if (!await Server.Instance.Init())
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

        private static void test()
        {
        }
    }
}
