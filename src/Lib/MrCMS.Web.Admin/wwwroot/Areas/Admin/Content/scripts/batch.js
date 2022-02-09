import {initTree} from "./tree-setup";

export function setupBatchFunctions() {
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
            $('[data-batch-run-progress-bar=' + id + ']').css('width', completionStatus.percentageCompleted);
            if ($('[data-batch-run-progress-bar=' + id + ']').length) {
                if (runInfo.status === 'Executing') {
                    $('title').text(completionStatus.percentageCompleted + ' Batch #' + id + ' - Executing');
                } else if (runInfo.status === 'Complete') {
                    $('title').text('Complete: Batch #' + id);
                }
            }
            $('[data-batch-run-full-status=' + id + ']').html(completionStatus.fullStatus);
            $('[data-batch-run-average-time-taken=' + id + ']').html(completionStatus.averageTimeTaken);
            $('[data-batch-run-time-taken=' + id + ']').html(completionStatus.timeTaken);
            $('[data-batch-run-pending=' + id + ']').html(completionStatus.pending);
            $('[data-batch-run-succeeded=' + id + ']').html(completionStatus.succeeded);
            $('[data-batch-run-time-remaining=' + id + ']').html(completionStatus.estimatedTimeRemaining);
            $('[data-batch-run-failed=' + id + ']').html(completionStatus.failed);
        }

        function updateJob(id) {
            var elements = $('[data-batch-job-id=' + id + ']');
            refreshElements(elements);
        }

        function init() {
            $(document).on('click', '[data-batch-run-start]', startBatch);
            $(document).on('click', '[data-batch-run-pause]', pauseBatch);
            var connection = new signalR.HubConnectionBuilder()
                .withUrl('/batchHub')
                .configureLogging(signalR.LogLevel.Warning)
                .build();
            connection.start().catch(err => console.error(err.toString()));
            connection.on('updateResult', updateResult);
            connection.on('updateRun', updateRun);
            connection.on('updateJob', updateJob);
            connection.on('refreshBatchRunUI', refreshRun);
        }

        return {
            init: init
        }
    };

    $('[data-batch-run]').each((index, element) => {
        new batchHandler().init();
    })
}