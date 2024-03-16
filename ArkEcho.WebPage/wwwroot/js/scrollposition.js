window.scrollFunctions = {
    getScrollPosition: function (elementId) {
        var element = document.getElementById(elementId);

        // On Firefox we get an int, on edge we get double... convert to int
        return {
            scrollTop: element.scrollTop|0,
            scrollLeft: element.scrollLeft|0
        };
    },
    setScrollPosition: function (elementId, scrollTop, scrollLeft) {
        var element = document.getElementById(elementId);
        element.scrollTop = scrollTop;
        element.scrollLeft = scrollLeft;
    }
};