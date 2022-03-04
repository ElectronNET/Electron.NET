import { HubConnection  } from "@microsoft/signalr";
import { Notification } from 'electron';
const notifications: Electron.Notification[] = (global['notifications'] = global['notifications'] || []) as Electron.Notification[];

export = (socket: HubConnection) => {
    socket.on('createNotification', (options) => {
        const notification = new Notification(options);
        let haveEvent = false;

        if (options.showID) {
            haveEvent = true;
            notification.on('show', () => {
                socket.invoke('NotificationEventOnShow', options.showID);
            });
        }

        if (options.clickID) {
            haveEvent = true;
            notification.on('click', () => {
                socket.invoke('NotificationEventOnClick', options.clickID);
            });
        }

        if (options.closeID) {
            haveEvent = true;
            notification.on('close', () => {
                socket.invoke('NotificationEventOnClose', options.closeID);
            });
        }

        if (options.replyID) {
            haveEvent = true;
            notification.on('reply', (event, value) => {
                socket.invoke('NotificationEventOnReply', [options.replyID, value]);
            });
        }

        if (options.actionID) {
            haveEvent = true;
            notification.on('action', (event, value) => {
                socket.invoke('NotificationEventOnAction', [options.actionID, value]);
            });
        }

        if (haveEvent) {
            notifications.push(notification);
        }

        notification.show();
    });

    socket.on('notificationIsSupported', (guid) => {
        const isSupported = Notification.isSupported;
        socket.invoke('SendClientResponseBool', guid, isSupported);
    });
};
