var TransientNotifications = function () {
    var showNotification = function (notification) {
        noty({
            text: notification.Date + ': ' + notification.Message,
            layout: 'bottomRight'
        });
    };
    return {
        init: function () {
            var adminHub = $.connection.notifications;
            adminHub.client.sendTransientNotification = showNotification;
            $.connection.hub.start().fail(function () { console.log('Could not Connect!'); });
        }
    };
};
$(function () {
    new TransientNotifications().init();
})