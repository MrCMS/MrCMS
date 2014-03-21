var TransientNotifications = function () {
    return {
        init: function () {
            var adminHub = $.connection.notifications;
            adminHub.client.sendTransientNotification = function (notification) {
                noty({
                    text: notification.Message,
                    layout: 'bottomRight'
                });
            };

            $.connection.hub.start().fail(function () { console.log('Could not Connect!'); });

        }
    };
};
$(function () {
    new TransientNotifications().init();
})