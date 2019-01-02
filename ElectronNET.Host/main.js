const { app } = require('electron');
const { BrowserWindow, dialog, shell } = require('electron');
const fs = require('fs');
const path = require('path');
const process = require('child_process').spawn;
const portscanner = require('portscanner');
let io, browserWindows, ipc, apiProcess, loadURL;
let appApi, menu, dialogApi, notification, tray, webContents;
let globalShortcut, shellApi, screen, clipboard;
let splashScreen, mainWindowId;

const manifestJsonFilePath = path.join(__dirname, 'bin', 'electron.manifest.json');
const manifestJsonFile = require(manifestJsonFilePath);
if (manifestJsonFile.singleInstance) {
    const shouldQuit = app.requestSingleInstanceLock();
    app.on('second-instance', (commandLine, workingDirectory) => {
        mainWindowId && BrowserWindow.fromId(mainWindowId) && BrowserWindow.fromId(mainWindowId).show();
    });

    if (shouldQuit) {
        app.quit();
    }
}

app.on('ready', () => {
    if (isSplashScreenEnabled()) {
        startSplashScreen();
    }

    portscanner.findAPortNotInUse(8000, 65535, '127.0.0.1', function (error, port) {
        console.log('Electron Socket IO Port: ' + port);
        startSocketApiBridge(port);
    });

});

function isSplashScreenEnabled() {
    return Boolean(manifestJsonFile.loadingUrl);
}

function startSplashScreen() {
    let loadingUrl = manifestJsonFile.loadingUrl;
    let icon = manifestJsonFile.icon;

    if (loadingUrl) {
        splashScreen = new BrowserWindow({
            width: manifestJsonFile.width,
            height: manifestJsonFile.height,
            transparent: true,
            frame: false,
            show: false,
            icon: path.join(__dirname, icon)
        });

        if (manifestJsonFile.devTools) {
            splashScreen.webContents.openDevTools();
        }

        splashScreen.loadURL(loadingUrl);
        splashScreen.once('ready-to-show', () => {
            splashScreen.show();
        });

        splashScreen.on('closed', () => {
            splashScreen = null;
        });
    }
}

function startSocketApiBridge(port) {

    // io = require('socket.io')(port); will trigger the windows firewall, but this doesn't work either:
    io = require('socket.io')("http://localhost:" + port);
    //startAspCoreBackend(port);
    //
    //io.on('connection', (socket) => {
    //    global['electronsocket'] = socket;
    //    global['electronsocket'].setMaxListeners(0);
    //    console.log('ASP.NET Core Application connected...', 'global.electronsocket', global['electronsocket'].id, new Date());
    //
    //    appApi = require('./api/app')(socket, app);
    //    browserWindows = require('./api/browserWindows')(socket, app);
    //    ipc = require('./api/ipc')(socket);
    //    menu = require('./api/menu')(socket);
    //    dialogApi = require('./api/dialog')(socket);
    //    notification = require('./api/notification')(socket);
    //    tray = require('./api/tray')(socket);
    //    webContents = require('./api/webContents')(socket);
    //    globalShortcut = require('./api/globalShortcut')(socket);
    //    shellApi = require('./api/shell')(socket);
    //    screen = require('./api/screen')(socket);
    //    clipboard = require('./api/clipboard')(socket);
    //
    //    if (splashScreen && !splashScreen.isDestroyed()) {
    //        splashScreen.close();
    //    }
    //});
}

function startAspCoreBackend(electronPort) {
    portscanner.findAPortNotInUse(8000, 65535, '127.0.0.1', function (error, electronWebPort) {
        console.log('ASP.NET Core Port: ' + electronWebPort);
        loadURL = `http://localhost:${electronWebPort}`;
        const parameters = [`/electronPort=${electronPort}`, `/electronWebPort=${electronWebPort}`];
        let binaryFile = manifestJsonFile.executable;

        const os = require('os');
        if (os.platform() === 'win32') {
            binaryFile = binaryFile + '.exe';
        }

        const binFilePath = path.join(__dirname, 'bin', binaryFile);
        var options = { cwd: path.join(__dirname, 'bin') };
        apiProcess = process(binFilePath, parameters, options);

        apiProcess.stdout.on('data', (data) => {
            console.log(`stdout: ${data.toString()}`);
        });
    });
}
