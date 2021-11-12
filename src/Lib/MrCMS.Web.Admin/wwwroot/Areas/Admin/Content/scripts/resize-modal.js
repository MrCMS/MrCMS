function resizeModal(jqElement) {
    if (!jqElement)
        return;
    if (jqElement.classList === undefined)
        return;
    
    const modal = jqElement.hasClass('modal') ? jqElement : jqElement.parents('.modal');
    const height = modal.outerHeight(),
        windowHeight = $(window).outerHeight(),
        width = modal.outerWidth(),
        windowWidth = $(window).outerWidth();
    const top = (windowHeight - height) / 2,
        left = (windowWidth - width) / 2;

    modal.css('top', top).css('left', left);
}

function hasClass(element, className) {
    return (' ' + element.className + ' ').indexOf(' ' + className+ ' ') > -1;
}

export function setupResizeModal() {
    $(window).resize(function () {
        $('.modal').each(function (index, element) {
            resizeModal($(element));
        });
    });
    window.resizeModal = resizeModal;
}
