function showNotification(notification) {
    noty({
        text: '<div class="notification-date">' + notification.date + '</div><div class="notification-msg">' + notification.message + "</div>",
        layout: 'bottomRight',
        timeout: 5000
    });
};

export function setupTransientNotifications() {
    let disableNotifications = $('body').data('disable-notifications');
    if (disableNotifications === "False") {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('/notificationsHub')
            .configureLogging(signalR.LogLevel.Warning)
            .build();
        connection.on('sendTransientNotification', showNotification);
        connection.start().catch(err => console.error(err.toString()));
    }
    
    
}
