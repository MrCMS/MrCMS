var TransientNotifications = function () {
    var showNotification = function (notification) {
        noty({
            text: '<div class="notification-date">' + notification.Date + '</div><div class="notification-msg">' + notification.Message + "</div>",
            layout: 'bottomRight',
            timeout: 5000
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