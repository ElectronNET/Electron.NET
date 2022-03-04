"use strict";
const electron_1 = require("electron");
let electronSocket;
;
module.exports = (socket) => {
    socket.on('desktop-capturer-get-sources', (options) => {
        electron_1.desktopCapturer.getSources(options).then(sources => {
            const result = sources.map(src => {
                return {
                    appIcon: src.appIcon,
                    name: src.name,
                    id: src.id,
                    display_id: src.display_id,
                    thumbnail: { 1: src.thumbnail.toPNG().toString('base64') }
                };
            });
            socket.invoke('SendClientResponseJObject', result);
        });
    });
};
//# sourceMappingURL=desktopCapturer.js.map