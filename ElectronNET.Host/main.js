const { app } = require('electron');
const fs = require('fs');
const path = require('path');
const process = require('child_process').spawn;
const portfinder = require('detect-port');
let io, browserWindows, ipc, apiProcess, loadURL, appApi, menu, dialog, notification, tray;

app.on('ready', () => {
    portfinder(8000, (error, port) => {
        startSocketApiBridge(port);
    });
});

function startSocketApiBridge(port) {
    io = require('socket.io')(port);
    startAspCoreBackend(port);

    io.on('connection', (socket) => {
        console.log('ASP.NET Core Application connected...');
        
        appApi = require('./api/app')(socket, app);
        browserWindows = require('./api/browserWindows')(socket);
        ipc = require('./api/ipc')(socket);        
        menu = require('./api/menu')(socket);
        dialog = require('./api/dialog')(socket);
        notification = require('./api/notification')(socket);
        tray = require('./api/tray')(socket);
    });
}

function startAspCoreBackend(electronPort) {
    portfinder(8000, (error, electronWebPort) => {
        loadURL = `http://localhost:${electronWebPort}`
        const parameters = [`/electronPort=${electronPort}`, `/electronWebPort=${electronWebPort}`];

        const manifestFile = require("./bin/electron.manifest.json");
        let binaryFile = manifestFile.executable;
        
        const os = require("os");
        if(os.platform() === "win32") {
            binaryFile = binaryFile + '.exe';
        }
        
        const binFilePath = path.join(__dirname, 'bin', binaryFile);
        apiProcess = process(binFilePath, parameters);

        apiProcess.stdout.on('data', (data) => {
            var text = data.toString();
            console.log(`stdout: ${data.toString()}`);
        });
    });
}

// Quit when all windows are closed.
app.on('window-all-closed', () => {
    // On macOS it is common for applications and their menu bar
    // to stay active until the user quits explicitly with Cmd + Q
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

//app.on('activate', () => {
    // On macOS it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
//    if (win === null) {
//        createWindow();
//    }
//});