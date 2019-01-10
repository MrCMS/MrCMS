var TransientNotifications = function () {
    var showNotification = function (notification) {
        noty({
            text: '<div class="notification-date">' + notification.date + '</div><div class="notification-msg">' + notification.message + "</div>",
            layout: 'bottomRight',
            timeout: 5000
        });
    };
    return {
        init: function () {
            var connection = new signalR.HubConnectionBuilder()
                .withUrl('/notificationsHub')
                .configureLogging(signalR.LogLevel.Warning)
                .build();
            connection.on('sendTransientNotification', showNotification);
            connection.start().catch(err => console.error(err.toString()));
        }
    };
};
$(function () {
    new TransientNotifications().init();
})