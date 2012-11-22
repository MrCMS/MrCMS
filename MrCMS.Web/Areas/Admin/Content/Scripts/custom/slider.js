$(function() {
    $('#add-slide-btn').click(function () {
        var i = $('#slides-table tbody tr').length;
        $('<tr>')
            .append($('<td>')
                .append($('<input>').attr('type', 'hidden').attr('id', 'Slides_' + i + '__Slider_Id').attr('name', 'Slides[' + i + '].Slider.Id').attr('value', $('#Id').val()))
                .append($('<input>').attr('type', 'hidden').attr('id', 'Slides_' + i + '__Id').attr('name', 'Slides[' + i + '].Id').attr('value', '0'))
                .append($('<input>').attr('type', 'text').attr('id', 'Slides_' + i + '__Title').attr('name', 'Slides[' + i + '].Title'))
            )
            .append($('<td>')
                .append($('<input>').attr('type', 'text').attr('id', 'Slides_' + i + '__Image').attr('name', 'Slides[' + i + '].Image').addClass('media-selector'))
            )
            .append($('<td>')
                .append($('<input>').attr('type', 'text').attr('id', 'Slides_' + i + '__Url').attr('name', 'Slides[' + i + '].Url'))
            )
            .append($('<td>')
                .append($('<input>').attr('type', 'checkbox').attr('id', 'Slides_' + i + '__Deleted').attr('name', 'Slides[' + i + '].Deleted').attr('value', 'true'))
                .append($('<input>').attr('type', 'hidden').attr('name', 'Slides[' + i + '].Deleted').attr('value', 'false'))
            ).appendTo($('#slides-table tbody'));
    });
})