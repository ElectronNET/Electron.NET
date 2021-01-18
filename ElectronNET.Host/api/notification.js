"use strict";
const electron_1 = require("electron");
const notifications = (global['notifications'] = global['notifications'] || []);
let electronSocket;
module.exports = (socket) => {
    electronSocket = socket;
    socket.on('createNotification', (options) => {
        const notification = new electron_1.Notification(options);
        let haveEvent = false;
        if (options.showID) {
            haveEvent = true;
            notification.on('show', () => {
                electronSocket.emit('NotificationEventShow', options.showID);
            });
        }
        if (options.clickID) {
            haveEvent = true;
            notification.on('click', () => {
                electronSocket.emit('NotificationEventClick', options.clickID);
            });
        }
        if (options.closeID) {
            haveEvent = true;
            notification.on('close', () => {
                electronSocket.emit('NotificationEventClose', options.closeID);
            });
        }
        if (options.replyID) {
            haveEvent = true;
            notification.on('reply', (event, value) => {
                electronSocket.emit('NotificationEventReply', [options.replyID, value]);
            });
        }
        if (options.actionID) {
            haveEvent = true;
            notification.on('action', (event, value) => {
                electronSocket.emit('NotificationEventAction', [options.actionID, value]);
            });
        }
        if (haveEvent) {
            notifications.push(notification);
        }
        notification.show();
    });
    socket.on('notificationIsSupported', () => {
        const isSupported = electron_1.Notification.isSupported;
        electronSocket.emit('notificationIsSupportedComplete', isSupported);
    });
};
//# sourceMappingURL=notification.js.map