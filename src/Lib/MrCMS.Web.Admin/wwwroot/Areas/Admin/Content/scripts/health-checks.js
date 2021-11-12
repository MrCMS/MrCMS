function showMessages(messagesCell, response) {
    if (response.messages.length) {
        const ul = $('<ul>').addClass('list-unstyled mb-0');
        $.each(response.messages, function (i, message) {
            $('<li>').text(message).appendTo(ul);
        });
        ul.appendTo(messagesCell);
    }
};

function setResult(row, statusCell, icon, rowClass) {
    statusCell.html('<i class="fa fa-' + icon + '"></i>');
    row.addClass(rowClass);
}

function onSuccess(row, statusCell) {
    setResult(row, statusCell, 'check', 'table-success');
    //statusCell.html('<i class="fa fa-ok"></i>');
    //row.addClass('success');
};

function onFailure(row, statusCell) {
    setResult(row, statusCell, 'remove', 'table-danger');
    //statusCell.html('<i class="fa fa-remove"></i>');
    //row.addClass('danger');
};

function onWarning(row, statusCell) {
    setResult(row, statusCell, 'asterisk', 'table-warning');
};

function onNA(row, statusCell) {
    setResult(row, statusCell, 'minus', 'table-info');
};

function processRow(row) {
    const url = row.data('process-url');
    const statusCell = row.find('[data-status]');
    const messagesCell = row.find('[ data-messages]');
    statusCell.html('Processing');
    $.get(url, function (response) {
        switch (response.status) {
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
}

export function setupHealthChecks() {
    $('[data-health-check-list]').find('[data-process-url]').each(function (index, element) {
        processRow($(element));
    });

}

//     };
//     return {
//         init: function () {
//             self = this;
//             element.find('[data-process-url]').each(function (index, element) {
//                 processRow($(element));
//             });
//         }
//     };
// };
//
//
// $(function () {
//     window.HealthCheckProcessor = new HealthCheckProcessor());
//     window.HealthCheckProcessor.init();
// });