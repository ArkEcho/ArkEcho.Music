using System;

namespace ArkEcho.Server
{
    public class TokenInstance
    {
        public TokenInstance(int userID)
        {
            UserID = userID;
        }

        public int UserID { get; }
        public Guid ApiToken { get; } = Guid.NewGuid();
        public DateTime Created { get; } = DateTime.Now;
    }
}
