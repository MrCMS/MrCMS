export function ajaxSetup() {
    let requestVerificationToken = $('body').data('antiforgery-token');
    $.ajaxSetup({
        cache: false,
        headers: {
            'RequestVerificationToken' : requestVerificationToken
        }        
    });
}
export function showHideLoading(){
    $(document).ajaxStart(function () {
        $("#loading").show();
    });
    $(document).ajaxStop(function () {
        $("#loading").hide();
    });
}
