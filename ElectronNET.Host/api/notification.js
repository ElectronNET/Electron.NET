"use strict";
exports.__esModule = true;
var electron_1 = require("electron");
module.exports = function (socket) {
    socket.on('createNotification', function (options) {
        var notification = new electron_1.Notification(options);
        notification.show();
    });
};
//# sourceMappingURL=notification.js.map