using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public interface IRest
    {
        int Timeout { get; set; }

        bool CheckConnection();
        Task<bool> PostLogging(LogMessage logMessage);
    }
}