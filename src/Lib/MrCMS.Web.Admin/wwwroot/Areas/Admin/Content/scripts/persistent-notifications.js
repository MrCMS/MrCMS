export function setupNotificationBar() {
    const PersistentNotifications = function () {
        const prependNotification = function (notification, updateLocalStorage = true) {
            listContainer.prepend(displayNotification(notification));
                if (updateLocalStorage) {
                    let notifications = getNotificationsLocalStorage();
                    notifications.push(notification);

                    while (notifications.length > 25) {
                        notifications.shift(); // Remove the oldest notification
                    }

                    updateNotificationsLocalStorage(notifications);
                    updateNotificationCountLocalStorage(notifications.length);
                }
                updateNotificationCount();
        };
        let displayNotification = function (notification) {
            return $('<li>').addClass('list-group-item').attr('data-notification', true).html('<div class="notification-date">' + notification.date + '</div><div class="notification-msg">' + notification.message + '</div>');
        };
        let resetNotifications = function () {
            listContainer.empty();
            listContainer.append(displayNoNotifications());
            updateNotificationCount();
        };
        const navContainer = $('#persistentNotificationLink');
        const listContainer = $('[data-notification-list]');

        // Local storage handling functions
        let updateNotificationCountLocalStorage = function (count) {
            localStorage.setItem('notificationCount', count);
        };

        let getNotificationCountLocalStorage = function () {
            return localStorage.getItem('notificationCount') || 0;
        };

        let updateNotificationsLocalStorage = function (notifications) {
            localStorage.setItem('notifications', JSON.stringify(notifications));
        };

        let getNotificationsLocalStorage = function () {
            return JSON.parse(localStorage.getItem('notifications')) || [];
        };

        // Existing functions
        const updateNotificationCount = function () {
            let count = getNotificationCountLocalStorage();
            $('[data-notification-count]').html(count);
        };

        const initializeNotifications = function () {
            let notifications = getNotificationsLocalStorage();
            listContainer.empty();
            if (notifications.length) {
                notifications.reverse();
                $.each(notifications, function (idx, notification) {
                    prependNotification(notification, false);
                });
            } else {
                resetNotifications();
            }
        };

        const displayNoNotifications = function () {
            return $('<li class="list-group-item">').html('No notifications');
        };

        const markAllAsRead = function () {
            // Clear the local storage data related to notifications
            localStorage.removeItem('notifications');
            localStorage.removeItem('notificationCount');

            // Reset the displayed notifications and count
            resetNotifications();
        };


        return {
            init: function () {
                updateNotificationCount();
                navContainer.on('hidden.bs.dropdown', function (e) {
                    markAllAsRead();
                });
                navContainer.on('show.bs.dropdown', function (e) {
                    initializeNotifications();
                });
                const connection = new signalR.HubConnectionBuilder()
                    .withUrl('/notificationsHub')
                    .configureLogging(signalR.LogLevel.Warning)
                    .build();

                connection.on('sendPersistentNotification', function (notification) {
                    prependNotification(notification);
                });

                connection.start().catch(err => console.error(err.toString()));
            }
        };
    };

    $(function () {
        let disableNotifications = $('body').data('disable-notifications');
        if (disableNotifications === "False") {
            new PersistentNotifications().init();
        }
    });
}
