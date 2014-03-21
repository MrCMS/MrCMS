var PersistentNotifications = function () {
    var initialized = false;
    var updateNotificationCount = function () {
        $('[data-notification-count]').html($('[data-notification-list] > [data-notification]').length);
    };
    var container = $('[data-notification-list]');
    var initializeNotifications = function () {
        $.get('/Admin/PersistentNotification/Get', function (notifications) {
            container.empty();
            if (notifications.length) {
                notifications.reverse();
                $.each(notifications, function (idx, notification) {
                    prependNotification(notification);
                });
                updateNotificationCount();
            } else {
                resetNotifications();
            }
            initialized = true;
        });
    };
    var displayNotification = function (notification) {
        return $('<li>').attr('data-notification', true).html(notification.Date + ': ' + notification.Message);
    };
    var displayNoNotifications = function () {
        return $('<li>').html('No notifications');
    };
    var prependNotification = function (notification) {
        container.prepend(displayNotification(notification));
        if (initialized) {
            updateNotificationCount();
        }
    };
    var resetNotifications = function () {
        container.empty();
        container.append(displayNoNotifications());
        updateNotificationCount();
    };
    var markAllAsRead = function (event) {
        event.preventDefault();
        $.post($(event.target).attr('action'), function () {
            initialized = false;
            initializeNotifications();
        });
    };
    return {
        init: function () {
            initializeNotifications();
            $(document).on('submit', '[data-mark-all-as-read]', markAllAsRead);
            var adminHub = $.connection.notifications;
            adminHub.client.sendPersistentNotification = prependNotification;
            $.connection.hub.start().fail(function () { console.log('Could not Connect!'); });
        }
    };
};
$(function () {
    new PersistentNotifications().init();
})