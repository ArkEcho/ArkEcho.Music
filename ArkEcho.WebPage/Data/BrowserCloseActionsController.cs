using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class BrowserCloseActionsController
    {
        private IJSRuntime runtime;
        private bool initialized;

        public event EventHandler CloseRequested;

        public BrowserCloseActionsController(IJSRuntime jsRuntime)
        {
            runtime = jsRuntime;
        }

        public async Task SetPageExit()
        {
            if (initialized)
                return;
            var dotnetRef = DotNetObjectReference.Create(this);
            await runtime.InvokeVoidAsync("setExitCheck", dotnetRef);
            initialized = true;
        }

        public async Task SetMessageOnPageExit(bool enable)
        {
            await runtime.InvokeVoidAsync("setMessageOnPageExit", enable);
        }

        [JSInvokable]
        public Task PageExit()
        {
            CloseRequested?.Invoke(null, EventArgs.Empty);
            return Task.CompletedTask;
        }
    }
}
