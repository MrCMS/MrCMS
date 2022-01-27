export function setupSimpleFiles(parent = document) {
    $(parent).find('.delete-file-simple').confirmation().on('click', function (e) {
        e.preventDefault();
        sendPostRequest(e.target);
    });
}

function sendPostRequest(el) {
    const url = $(el).data('value');
    $.ajax({
        type: "POST",
        url: url,
        success: function () {
            const fileList = $(document).find('#file-list-simple');
            if (fileList) {
                $.get('/Admin/MediaCategory/ShowFilesSimple/' + fileList.data('category-id'), function (response) {
                    $(fileList).replaceWith(response);
                    setupSimpleFiles('#file-list-simple');
                });
            }
        }
    });
}