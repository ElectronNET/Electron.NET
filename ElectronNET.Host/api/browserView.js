"use strict";
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var electron_1 = require("electron");
var browserViews = (global['browserViews'] = global['browserViews'] || []);
var browserView, electronSocket;
module.exports = function (socket) {
    electronSocket = socket;
    socket.on('createBrowserView', function (options) {
        if (!hasOwnChildreen(options, 'webPreferences', 'nodeIntegration')) {
            options = __assign(__assign({}, options), { webPreferences: { nodeIntegration: true } });
        }
        browserView = new electron_1.BrowserView(options);
        browserViews.push(browserView);
        electronSocket.emit('BrowserViewCreated', browserView.id);
    });
    socket.on('browserView-isDestroyed', function (id) {
        var isDestroyed = getBrowserViewById(id).isDestroyed();
        electronSocket.emit('browserView-isDestroyed-reply', isDestroyed);
    });
    socket.on('browserView-getBounds', function (id) {
        var bounds = getBrowserViewById(id).getBounds();
        electronSocket.emit('browserView-getBounds-reply', bounds);
    });
    socket.on('browserView-setBounds', function (id, bounds) {
        getBrowserViewById(id).setBounds(bounds);
    });
    socket.on('browserView-destroy', function (id) {
        var browserViewIndex = browserViews.findIndex(function (b) { return b.id === id; });
        getBrowserViewById(id).destroy();
        browserViews.splice(browserViewIndex, 1);
    });
    socket.on('browserView-setAutoResize', function (id, options) {
        getBrowserViewById(id).setAutoResize(options);
    });
    socket.on('browserView-setBackgroundColor', function (id, color) {
        getBrowserViewById(id).setBackgroundColor(color);
    });
    function hasOwnChildreen(obj) {
        var childNames = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            childNames[_i - 1] = arguments[_i];
        }
        for (var i = 0; i < childNames.length; i++) {
            if (!obj || !obj.hasOwnProperty(childNames[i])) {
                return false;
            }
            obj = obj[childNames[i]];
        }
        return true;
    }
    function getBrowserViewById(id) {
        for (var index = 0; index < browserViews.length; index++) {
            var browserViewItem = browserViews[index];
            if (browserViewItem.id === id) {
                return browserViewItem;
            }
        }
    }
};
