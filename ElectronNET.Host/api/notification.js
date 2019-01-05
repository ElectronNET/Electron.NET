"use strict";
const electron_1 = require("electron");
const notifications = [];
module.exports = (socket) => {
    socket.on('createNotification', (options) => {
        const notification = new electron_1.Notification(options);
        let haveEvent = false;
        if (options.showID) {
            haveEvent = true;
            notification.on('show', () => {
                socket.emit('NotificationEventShow', options.showID);
            });
        }
        if (options.clickID) {
            haveEvent = true;
            notification.on('click', () => {
                socket.emit('NotificationEventClick', options.clickID);
            });
        }
        if (options.closeID) {
            haveEvent = true;
            notification.on('close', () => {
                socket.emit('NotificationEventClose', options.closeID);
            });
        }
        if (options.replyID) {
            haveEvent = true;
            notification.on('reply', (event, value) => {
                socket.emit('NotificationEventReply', [options.replyID, value]);
            });
        }
        if (options.actionID) {
            haveEvent = true;
            notification.on('action', (event, value) => {
                socket.emit('NotificationEventAction', [options.actionID, value]);
            });
        }
        if (haveEvent) {
            notifications.push(notification);
        }
        notification.show();
    });
    socket.on('notificationIsSupported', (options) => {
        const isSupported = electron_1.Notification.isSupported;
        socket.emit('notificationIsSupportedComplete', isSupported);
    });
};
//# sourceMappingURL=notification.js.map