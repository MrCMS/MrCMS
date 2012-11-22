$(function () {
    $('#add-gallery-item-btn').click(function () {
        var i = $('#gallery-items-table tbody tr').length;
        $('<tr>')
            .append($('<td>')
                .append($('<input>').attr('type', 'hidden').attr('id', 'Items_' + i + '__Slider_Id').attr('name', 'Items[' + i + '].Gallery.Id').attr('value', $('#Id').val()))
                .append($('<input>').attr('type', 'hidden').attr('id', 'Items_' + i + '__Id').attr('name', 'Items[' + i + '].Id').attr('value', '0'))
                .append($('<input>').attr('type', 'text').attr('id', 'Items_' + i + '__Preview').attr('name', 'Items[' + i + '].Preview').addClass('media-selector'))
            )
            .append($('<td>')
                .append($('<input>').attr('type', 'text').attr('id', 'Items_' + i + '__FullImage').attr('name', 'Items[' + i + '].FullImage').addClass('media-selector'))
            )
            .append($('<td>')
                .append($('<input>').attr('type', 'text').attr('id', 'Items_' + i + '__DisplayOrder').attr('name', 'Items[' + i + '].DisplayOrder').addClass('order').val(i + 1))
            )
            .append($('<td>')
                .append($('<input>').attr('type', 'checkbox').attr('id', 'Items_' + i + '__Deleted').attr('name', 'Items[' + i + '].Deleted').attr('value', 'true'))
                .append($('<input>').attr('type', 'hidden').attr('name', 'Items[' + i + '].Deleted').attr('value', 'false'))
            ).appendTo($('#gallery-items-table tbody'));
    });
})