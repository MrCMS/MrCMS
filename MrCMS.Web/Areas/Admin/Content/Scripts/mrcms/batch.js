(function ($) {
    var batchHandler = function () {
        function refreshRun(id) {
            var elements = $('[data-batch-run-id=' + id + ']');
            refreshElements(elements);
        }
        function startBatch(event) {
            var batchRunId = $(event.target).data('batch-run-id');
            $.post('/admin/batchrun/start/' + batchRunId, function () {
                refreshRun(batchRunId);
            });
        }
        function pauseBatch(event) {
            var batchRunId = $(event.target).data('batch-run-id');
            $.post('/admin/batchrun/pause/' + batchRunId, function () {
                refreshRun(batchRunId);
            });
        }
        function refreshElements(elements) {
            elements.each(function (index, el) {
                var element = $(el);
                var refreshUrl = element.data('refresh-url');
                if (refreshUrl) {
                    $.get(refreshUrl, function (response) {
                        element.replaceWith(response);
                    });
                }
            });
        }

        function updateResult(id) {
            var elements = $('[data-batch-run-result-id=' + id + ']');
            refreshElements(elements);
        }
        function updateRun(runInfo) {
            var id = runInfo.id;
            $('[data-batch-run-status=' + id + ']').html(runInfo.status);
            var completionStatus = runInfo.completionStatus;
            $('[data-batch-run-progress-bar=' + id + ']').css('width', completionStatus.PercentageCompleted);
            if ($('[data-batch-run-progress-bar=' + id + ']').length) {
                if (runInfo.status == 'Executing') {
                    $('title').text(completionStatus.PercentageCompleted + ' Batch #' + id + ' - Executing');
                } else if (runInfo.status == 'Complete') {
                    $('title').text('Complete: Batch #' + id);
                }
            }
            $('[data-batch-run-full-status=' + id + ']').html(completionStatus.FullStatus);
            $('[data-batch-run-average-time-taken=' + id + ']').html(completionStatus.AverageTimeTaken);
            $('[data-batch-run-time-taken=' + id + ']').html(completionStatus.TimeTaken);
            $('[data-batch-run-pending=' + id + ']').html(completionStatus.Pending);
            $('[data-batch-run-succeeded=' + id + ']').html(completionStatus.Succeeded);
            $('[data-batch-run-failed=' + id + ']').html(completionStatus.Failed);
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
            batchHub.client.refreshBatchRunUI = refreshRun;
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