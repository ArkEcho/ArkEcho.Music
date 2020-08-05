using System;
using System.Runtime.Serialization;

namespace ArkEcho.Core
{
    [DataContract]
    [Serializable]
    public class MusicFile
    {
        [DataMember]
        public long ID { get; set; } = 0;

        public string Title { get; set; } = string.Empty;

        public MusicFile()
        {
            
        }
    }
}
