import { Notification } from "electron";
const notifications: Electron.Notification[] = [];

module.exports = (socket: SocketIO.Server) => {
    socket.on('createNotification', (options) => {
        const notification = new Notification(options);
        let haveEvent = false;

        if(options.showID) {
            haveEvent = true;
            notification.on('show', () => {
                socket.emit('NotificationEventShow', options.showID);
            });
        }

        if(options.clickID) {
            haveEvent = true;
            notification.on('click', () => {
                socket.emit('NotificationEventClick', options.clickID);
            });
        }

        if(options.closeID) {
            haveEvent = true;
            notification.on('close', () => {
                socket.emit('NotificationEventClose', options.closeID);
            });
        }

        if(options.replyID) {
            haveEvent = true;
            notification.on('reply', (event, value) => {
                socket.emit('NotificationEventReply', [options.replyID, value]);
            });
        }

        if(options.actionID) {
            haveEvent = true;
            notification.on('action', (event, value) => {
                socket.emit('NotificationEventAction', [options.actionID, value]);
            });
        }

        if(haveEvent) {
            notifications.push(notification);
        }

        notification.show();
    });

    socket.on('notificationIsSupported', (options) => {
        const isSupported = Notification.isSupported;
        socket.emit('notificationIsSupportedComplete', isSupported);
    });
}