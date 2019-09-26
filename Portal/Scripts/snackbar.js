showSnackbar = (function () {
    var queue = [];
    var isPlaying = false;

    function showSnackbar(msg) {
        queue.push(msg);
        if (!isPlaying) {
            showNext();
        }
    }

    function showNext() {
        var elem = document.getElementById("snackbar");
        elem.classList.remove('show');
        void elem.offsetWidth;
        if (queue.length > 0) {
            isPlaying = true;
            elem.textContent = queue.pop();
            elem.classList.add('show');
            setTimeout(showNext, 3000);
        } else {
            isPlaying = false;
        }
    }

    return showSnackbar;
})();