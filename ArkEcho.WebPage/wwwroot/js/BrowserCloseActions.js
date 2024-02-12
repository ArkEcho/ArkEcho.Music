
var blazorInterop;
var enableMessageBoxOnPageExit = false;

setExitCheck = function (dotnetInterop) {
    window.addEventListener("beforeunload", pageExit);
    blazorInterop = dotnetInterop;
    console.log('SetExitCheck');
}

setMessageOnPageExit = function (enable) {
    enableMessageBoxOnPageExit = enable;
    console.log('Page Exit Message ' + enable);
}

pageExit = function (event) {
    if (enableMessageBoxOnPageExit) {
        event.preventDefault();
    }

    blazorInterop.invokeMethodAsync("PageExit");
}