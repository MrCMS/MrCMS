(function ($) {
    var BatchHandler = function () {
        function startBatch(event) {
            var button = $(event.target);
            var batchRunId = button.data('batch-run-id');
            $.post('/admin/batchrun/start/' + batchRunId, function () {
                var element = button.parents('[data-update-batch]');
                refreshElement(element);
            });
        }
        function pauseBatch(event) {
            var batchRunId = $(event.target).data('batch-run-id');
            $.post('/admin/batchrun/pause/' + batchRunId, function () {

            });
        }
        function refreshElement(element) {
            var refreshUrl = element.data('refresh-url');
            if (refreshUrl) {
                $.get(refreshUrl, function (response) {
                    var $response = $(response);
                    $(element).empty();
                    $(element).append($response.children());
                });
            }
        }
        function startPolling(element) {
            setInterval(function () {
                var status = $(element).find('[data-status]').text().trim();
                if (status !== 'Executing') {
                    return;
                }
                refreshElement($(element));
            }, 1000);
        }

        function init() {
            $(document).on('click', '[data-batch-run-start]', startBatch);
            $(document).on('click', '[data-batch-run-pause]', pauseBatch);
            $('[data-update-batch]').each(function (index, element) {
                startPolling(element);
            });
        }
        return {
            init: init
        }
    };

    $(function () {
        new BatchHandler().init();
    });
})(jQuery);