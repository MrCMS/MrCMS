export function setupNotificationBar() {
    var PersistentNotifications = function () {
        var initialized = false;
        var navContainer = $('#persistentNotificationLink');
        var updateNotificationCount = function () {
            $.get("/Admin/PersistentNotification/GetCount", function (data) {
                $('[data-notification-count]').html(data);
            });
        };
        var listContainer = $('[data-notification-list]');
        var initializeNotifications = function () {
            $.get('/Admin/PersistentNotification/Get', function (notifications) {
                listContainer.empty();
                if (notifications.length) {
                    notifications.reverse();
                    $.each(notifications, function (idx, notification) {
                        prependNotification(notification);
                    });
                } else {
                    resetNotifications();
                }
                initialized = true;
            });
        };
        var displayNotification = function (notification) {
            return $('<li>').addClass('list-group-item').attr('data-notification', true).html('<div class="notification-date">' + notification.date + '</div><div class="notification-msg">' + notification.message + '</div>');
        };
        var displayNoNotifications = function () {
            return $('<li class=\"list-group-item\">').html('No notifications');
        };
        var prependNotification = function (notification) {
            listContainer.prepend(displayNotification(notification));   
            if (initialized) {
                updateNotificationCount();
            }
        };
        var resetNotifications = function () {
            listContainer.empty();
            listContainer.append(displayNoNotifications());
            updateNotificationCount();
        };
        var markAllAsRead = function () {
            $.post(navContainer.data('mark-as-read-url'), function () {
                initialized = false;
                initializeNotifications();
            });
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
                var connection = new signalR.HubConnectionBuilder()
                    .withUrl('/notificationsHub')
                    .configureLogging(signalR.LogLevel.Warning)
                    .build();
                connection.on('sendPersistentNotification', prependNotification);
                connection.start().catch(err => console.error(err.toString()));
            }
        };
        
       
    };
    $(function () {
        let disableNotifications = $('body').data('disable-notifications');
        if (disableNotifications === "False") {
            new PersistentNotifications().init();
        }
    })
}