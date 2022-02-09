import {postToUrl} from "./post-to-url";

export function setupConfirmation() {
    
    
    $('[data-toggle=confirmation]').confirmation().on('click', function (e) {
        e.preventDefault();
        deleteFromConfirmation(e.target);
    });

    function deleteFromConfirmation(el) {
        const deleteUrl = $(el).data('value');
        const id = $(el).data('id');
        postToUrl(deleteUrl, {id: id});
    }
}
