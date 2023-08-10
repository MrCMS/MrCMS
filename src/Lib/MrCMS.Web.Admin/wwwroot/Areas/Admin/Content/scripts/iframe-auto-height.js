
export function initIFrameAutoHeight() {
    $('iframe[auto-height]').on('load', function (e) {
        let obj = e.target;
        obj.style.height = obj.contentWindow.document.documentElement.scrollHeight + 'px';
        $(obj).attr('auto-height', true);
    });

    setTimeout(()=>{
        $('iframe[auto-height]').each((index, element) => {
            if (!$(element).attr('auto-height')) {
                element.style.height = element.contentWindow.document.documentElement.scrollHeight + 'px';
                $(element).attr('auto-height', true);
                element.src += '';
            }
        });
    }, 1000);
}