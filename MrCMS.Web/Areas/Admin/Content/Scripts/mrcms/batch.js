(function ($) {
    var batchHandler = function () {
        function responseHandler(response) {
            if (response) {
                executeNext(response);
            }
        }
        function executeNext(batchRunId) {
            $.post('/admin/batchrun/executenext/' + batchRunId, responseHandler);
        }
        function startBatch(event) {
            var batchRunId = $(event.target).data('batch-run-id');
            $.post('/admin/batchrun/start/' + batchRunId, responseHandler);
        }
        function pauseBatch(event) {
            var batchRunId = $(event.target).data('batch-run-id');
            $.post('/admin/batchrun/pause/' + batchRunId);
        }
        function refreshElements(elements) {
            elements.each(function (index, el) {
                var element = $(el);
                var refreshUrl = element.data('refresh-url');
                if (refreshUrl) {
                    var refresh = element.data('refresh');
                    if (refresh)
                        clearTimeout(refresh);
                    element.data('refresh', setTimeout(function() {
                        $.get(refreshUrl, function(response) {
                            element.replaceWith(response);
                        });
                    }, 300));
                }
            });
        }

        function updateResult(id) {
            var elements = $('[data-batch-run-result-id=' + id + ']');
            refreshElements(elements);
        }
        function updateRun(id) {
            var elements = $('[data-batch-run-id=' + id + ']');
            refreshElements(elements);
        }
        function updateJob(id) {
            var elements = $('[data-batch-job-id=' + id + ']');
            refreshElements(elements);
        }

        function init() {
            $(document).on('click', '[data-batch-run-start]', startBatch);
            $(document).on('click', '[data-batch-run-pause]', pauseBatch);
            var batchHub = $.connection.batch;
            batchHub.client.updateResult = updateResult;
            batchHub.client.updateRun = updateRun;
            batchHub.client.updateJob = updateJob;
            $.connection.hub.start().fail(function () { console.log('Could not Connect!'); });
        }
        return {
            init: init
        }
    };

    $(function () {
        new batchHandler().init();
    });
})(jQuery);