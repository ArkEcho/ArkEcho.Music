using ArkEcho.Core;

namespace ArkEcho.Desktop
{
    public class DesktopLibrarySync : LibarySyncBase
    {
        public DesktopLibrarySync(Rest rest, RestLoggingWorker loggingWorker) : base(rest)
        {
            logger = new Logger(ArkEcho.Resources.ARKECHODESKTOP, "MusicSync", loggingWorker);
        }
    }
}
