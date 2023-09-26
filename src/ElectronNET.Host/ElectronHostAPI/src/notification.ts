import { Notification } from 'electron';
import { Socket } from 'node:net';

const notifications: Electron.Notification[] = (global['notifications'] = global['notifications'] || []) as Electron.Notification[];
let electronSocket;

export function notificationApi(socket: Socket) {
    electronSocket = socket;
    socket.on('createNotification', (options) => {
        const notification = new Notification(options);
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
        const isSupported = Notification.isSupported;
        electronSocket.emit('notificationIsSupportedComplete', isSupported);
    });
}
