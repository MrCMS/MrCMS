export function initializeStickyCKEditor() {
    //fix ckeditor on scroll
    $("[data-main-content]").scroll((e) => {
        setStickyCkeditor(this);
    });
}

function setStickyCkeditor(el) {
    setTimeout(() => {
        if ($('[data-body-content] #cke_1_contents').height() > 500) {
            if ($(el).scrollTop() > 110 && $("[data-body-content] #cke_1_top").css('position') != 'fixed') {
                $("[data-body-content] #cke_1_top").css({'position': 'fixed', 'top': '51px'});
            }
            if ($(el).scrollTop() < 110 && $("[data-body-content] #cke_1_top").css('position') != 'inherit') {
                $("[data-body-content] #cke_1_top").css({'position': 'inherit', 'top': 'auto;'});
            }
        } else {
            $("[data-body-content] #cke_1_top").css({'position': 'inherit', 'top': 'auto;'});
        }
    }, 250);
}
