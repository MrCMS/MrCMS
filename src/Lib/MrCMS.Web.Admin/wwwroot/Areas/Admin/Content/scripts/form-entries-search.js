$(function () {

    $(document).on('keypress',
        function (event) {
            if (event.which === 13) {
                if ($('input#Search:focus').length) {
                    event.preventDefault();
                    $('#search-postings').click();
                }
            }
        });

});
