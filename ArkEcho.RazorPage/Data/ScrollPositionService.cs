using Microsoft.JSInterop;

namespace ArkEcho.RazorPage.Data
{
    public class ScrollPositionService
    {
        private class ScrollPosition
        {
            public int ScrollTop { get; set; }
            public int ScrollLeft { get; set; }
        }

        private IJSRuntime jsRuntime;
        private Dictionary<string, ScrollPosition> scrollPositions = new Dictionary<string, ScrollPosition>();

        public ScrollPositionService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task SaveScrollPosition(string elementID)
        {
            scrollPositions[elementID] = await jsRuntime.InvokeAsync<ScrollPosition>("scrollFunctions.getScrollPosition", new object[] { elementID });
        }

        public async Task LoadScrollPosition(string elementID)
        {
            if (scrollPositions.TryGetValue(elementID, out ScrollPosition sp))
                await jsRuntime.InvokeVoidAsync("scrollFunctions.setScrollPosition", elementID, sp.ScrollTop, sp.ScrollLeft);
        }
    }
}
