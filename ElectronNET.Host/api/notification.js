"use strict";
var electron_1 = require("electron");
var notifications = (global['notifications'] = global['notifications'] || []);
var electronSocket;
module.exports = function (socket) {
    electronSocket = socket;
    socket.on('createNotification', function (options) {
        var notification = new electron_1.Notification(options);
        var haveEvent = false;
        if (options.showID) {
            haveEvent = true;
            notification.on('show', function () {
                electronSocket.emit('NotificationEventShow', options.showID);
            });
        }
        if (options.clickID) {
            haveEvent = true;
            notification.on('click', function () {
                electronSocket.emit('NotificationEventClick', options.clickID);
            });
        }
        if (options.closeID) {
            haveEvent = true;
            notification.on('close', function () {
                electronSocket.emit('NotificationEventClose', options.closeID);
            });
        }
        if (options.replyID) {
            haveEvent = true;
            notification.on('reply', function (event, value) {
                electronSocket.emit('NotificationEventReply', [options.replyID, value]);
            });
        }
        if (options.actionID) {
            haveEvent = true;
            notification.on('action', function (event, value) {
                electronSocket.emit('NotificationEventAction', [options.actionID, value]);
            });
        }
        if (haveEvent) {
            notifications.push(notification);
        }
        notification.show();
    });
    socket.on('notificationIsSupported', function () {
        var isSupported = electron_1.Notification.isSupported;
        electronSocket.emit('notificationIsSupportedComplete', isSupported);
    });
};
