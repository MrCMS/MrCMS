var HealthCheckProcessor = function (element) {
    var self;
    var showMessages = function (messagesCell, response) {
        if (response.Messages.length) {
            var ul = $('<ul>').addClass('list-unstyled margin-bottom-0');
            $.each(response.Messages, function (i, message) {
                $('<li>').text(message).appendTo(ul);
            });
            ul.appendTo(messagesCell);
        }
    };
    function setResult(row, statusCell, icon, rowClass) {
        statusCell.html('<i class="glyphicon glyphicon-' + icon + '"></i>');
        row.addClass(rowClass);
    }
    var onSuccess = function (row, statusCell) {
        setResult(row, statusCell, 'ok', 'success');
        //statusCell.html('<i class="glyphicon glyphicon-ok"></i>');
        //row.addClass('success');
    };
    var onFailure = function (row, statusCell) {
        setResult(row, statusCell, 'remove', 'danger');
        //statusCell.html('<i class="glyphicon glyphicon-remove"></i>');
        //row.addClass('danger');
    };
    var onWarning = function (row, statusCell) {
        setResult(row, statusCell, 'asterisk', 'warning');
    };
    var onNA = function (row, statusCell) {
        setResult(row, statusCell, 'minus', 'info');
    };
    var processRow = function (row) {
        var url = row.data('process-url');
        var statusCell = row.find('[data-status]');
        var messagesCell = row.find('[ data-messages]');
        statusCell.html('Processing');
        $.get(url, function (response) {
            switch (response.Status) {
                case 0:
                    onSuccess(row, statusCell);
                    break;
                case 1:
                    onFailure(row, statusCell);
                    break;
                case 2:
                    onWarning(row, statusCell);
                    break;
                case 3:
                    onNA(row, statusCell);
                    break;
            }
            showMessages(messagesCell, response);
        });
    };
    return {
        init: function () {
            self = this;
            element.find('[data-process-url]').each(function (index, element) {
                processRow($(element));
            });
        }
    };
};


$(function () {
    window.HealthCheckProcessor = new HealthCheckProcessor($('[data-health-check-list]'));
    window.HealthCheckProcessor.init();
})