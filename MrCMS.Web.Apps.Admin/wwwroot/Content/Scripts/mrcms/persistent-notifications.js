var PersistentNotifications = function () {
    var initialized = false;
    var updateNotificationCount = function () {
        $.get("/Admin/PersistentNotification/GetCount", function (data) {
            $('[data-notification-count]').html(data);
        });
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
        return $('<li>').attr('data-notification', true).html('<div class="notification-date">' + notification.Date + '</div><div class="notification-msg">' + notification.Message + '</div>');
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