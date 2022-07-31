using ArkEcho.Core;

namespace ArkEcho.App
{
    public class LibrarySyncAndroid : LibarySyncBase
    {
        public LibrarySyncAndroid(Rest rest) : base(rest)
        {
            logger = new Logger(ArkEcho.Resources.ARKECHOAPP, "MusicSync", AppModel.Instance.RestLoggingWorker);
        }
    }
}
