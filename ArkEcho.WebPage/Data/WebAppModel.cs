using ArkEcho.Core;
using ArkEcho.RazorPage.Data;

namespace ArkEcho.WebPage
{
    public class WebAppModel : AppModelBase
    {
        public WebAppModel(Rest rest, Logger logger)
            : base(logger, rest)
        {
        }
    }
}
