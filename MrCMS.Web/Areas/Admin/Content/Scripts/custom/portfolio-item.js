$(function () {
    $('#add-image-btn').click(function () {
        var i = $('#images-table tbody tr').length;
        $('<tr>')
            .append($('<td>')
                .append($('<input>').attr('type', 'hidden').attr('id', 'Images_' + i + '__PortfolioItem_Id').attr('name', 'Images[' + i + '].PortfolioItem.Id').attr('value', $('#Id').val()))
                .append($('<input>').attr('type', 'hidden').attr('id', 'Images_' + i + '__Id').attr('name', 'Images[' + i + '].Id').attr('value', '0'))
                .append($('<input>').attr('type', 'text').attr('id', 'Images_' + i + '__Title').attr('name', 'Images[' + i + '].Title'))
            )
            .append($('<td>')
                .append($('<input>').attr('type', 'text').attr('id', 'Images_' + i + '__Image').attr('name', 'Images[' + i + '].Image').addClass('media-selector'))
            )
            .append($('<td>')
                .append($('<input>').attr('type', 'checkbox').attr('id', 'Images_' + i + '__IsPrimaryImage').attr('name', 'Images[' + i + '].IsPrimaryImage').attr('value', 'true'))
                .append($('<input>').attr('type', 'hidden').attr('name', 'Images[' + i + '].IsPrimaryImage').attr('value', 'false'))
            )
            .append($('<td>')
                .append($('<input>').attr('type', 'checkbox').attr('id', 'Images_' + i + '__Deleted').attr('name', 'Images[' + i + '].Deleted').attr('value', 'true'))
                .append($('<input>').attr('type', 'hidden').attr('name', 'Images[' + i + '].Deleted').attr('value', 'false'))
            ).appendTo($('#images-table tbody'));
    });
});