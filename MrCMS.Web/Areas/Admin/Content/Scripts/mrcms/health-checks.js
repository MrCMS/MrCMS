var HealthCheckProcessor = function (element) {
    var self;
    var showMessages = function (messagesCell, response) {
        if (response.Messages.length) {
            var ul = $('<ul>');
            $.each(response.Messages, function (i, message) {
                $('<li>').text(message).appendTo(ul);
            });
            ul.appendTo(messagesCell);
        }
    };
    var onSuccess = function (row, statusCell) {
        statusCell.html('<i class="glyphicon glyphicon-ok"></i>');
        row.addClass('success');
    };
    var onFailure = function (row, statusCell) {
        statusCell.html('<i class="glyphicon glyphicon-remove"></i>');
        row.addClass('danger');
    };
    var processRow = function (row) {
        var url = row.data('process-url');
        var statusCell = row.find('[data-status]');
        var messagesCell = row.find('[ data-messages]');
        statusCell.html('Processing');
        $.get(url, function (response) {
            response.OK ? onSuccess(row, statusCell) : onFailure(row, statusCell);
            showMessages(messagesCell, response);
            var next = row.next();
            if (next.length) {
                processRow(next);
            }
        });
    };
    return {
        init: function () {
            self = this;
            processRow(element.find('[data-process-url]').eq(0));
        }
    };
};


$(function () {
    window.HealthCheckProcessor = new HealthCheckProcessor($('[data-health-check-list]'));
    window.HealthCheckProcessor.init();
})