export function registerDelayKeyup() {
    $.fn.delayKeyup = function (e, callback, ms) {
        let timer = 0;
        $(this).keyup(function (event) {
            clearTimeout(timer);
            timer = setTimeout(function () {
                callback(event);
            }, ms);
        });
        return $(this);
    };
}
