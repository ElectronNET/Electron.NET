"use strict";
exports.__esModule = true;
module.exports = function (socket, app) {
    socket.on('appQuit', function () {
        app.quit();
    });
    socket.on('appExit', function (exitCode) {
        if (exitCode === void 0) { exitCode = 0; }
        app.exit(exitCode);
    });
    socket.on('appRelaunch', function (options) {
        app.relaunch(options);
    });
    socket.on('appFocus', function () {
        app.focus();
    });
    socket.on('appHide', function () {
        app.hide();
    });
    socket.on('appShow', function () {
        app.show();
    });
    socket.on('appGetAppPath', function () {
        var path = app.getAppPath();
        socket.emit('appGetAppPathCompleted', path);
    });
    socket.on('appGetPath', function (name) {
        var path = app.getPath(name);
        socket.emit('appGetPathCompleted', path);
    });
};
//# sourceMappingURL=app.js.map