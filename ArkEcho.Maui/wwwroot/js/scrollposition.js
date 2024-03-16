window.scrollFunctions = {
    getScrollPosition: function (elementId) {
        var element = document.getElementById(elementId);
        return {
            scrollTop: element.scrollTop,
            scrollLeft: element.scrollLeft
        };
    },
    setScrollPosition: function (elementId, scrollTop, scrollLeft) {
        var element = document.getElementById(elementId);
        element.scrollTop = scrollTop;
        element.scrollLeft = scrollLeft;
    }
};