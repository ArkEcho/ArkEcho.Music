using ArkEcho.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public class ArkEchoServer
    {
        // Singleton Implementation
        public static ArkEchoServer Server = new ArkEchoServer();

        private List<MusicFile> files = null;

        private ArkEchoServer()
        {
            files = new List<MusicFile>();
        }

        public bool Init()
        {
            files.Add(new MusicFile() { ID = 1, Title = "Tick" });
            files.Add(new MusicFile() { ID = 2, Title = "Trick" });
            files.Add(new MusicFile() { ID = 3, Title = "Track" });

            return true;
        }

        public List<MusicFile> GetAllFiles()
        {
            return files;
        }
    }
}
