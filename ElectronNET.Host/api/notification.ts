import { Notification } from "electron";

module.exports = (socket: SocketIO.Server) => {
    socket.on('createNotification', (options) => {
        const notification = new Notification(options);
        notification.show();
    });
}