using System;

namespace ArkEcho.Server
{
    public class TokenInstance
    {
        public Guid ApiToken { get; } = Guid.NewGuid();
        public DateTime Created { get; } = DateTime.Now;
    }
}
