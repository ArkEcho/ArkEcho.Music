using ArkEcho.Core;
using ArkEcho.RazorPage.Data;

namespace ArkEcho.WebPage
{
    public class WebLibraryController : LibraryControllerBase
    {
        public WebLibraryController(Rest rest, Logger logger)
            : base(logger, rest)
        {
        }
    }
}
