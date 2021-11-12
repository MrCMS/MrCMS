import {postToUrl} from './post-to-url'
export function registerActions(document){
    $(document).on('click', '[data-action=save]', (e) => {
        e.preventDefault();
        const formId = $(this).data('form-id');
        $('#' + formId).submit();
    });

    $(document).on('click', '[data-action=post-link]', (e) => {
        e.preventDefault();
        const self = $(this);
        const url = self.attr('href') || self.data('link');
        if (url != null) {
            postToUrl(url, {});
        }
    });
}