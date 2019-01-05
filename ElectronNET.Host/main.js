const { app } = require('electron');
const { BrowserWindow, dialog, shell } = require('electron');
const fs = require('fs');
const path = require('path');
const process = require('child_process').spawn;
const portscanner = require('portscanner');
let io, server, browserWindows, ipc, apiProcess, loadURL;
let appApi, menu, dialogApi, notification, tray, webContents;
let globalShortcut, shellApi, screen, clipboard;
let splashScreen, mainWindowId, hostHook;

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

    // hostname needs to belocalhost, otherwise Windows Firewall will be triggered.
    portscanner.findAPortNotInUse(8000, 65535, 'localhost', function (error, port) {
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

    // instead of 'require('socket.io')(port);' we need to use this workaround
    // otherwise the Windows Firewall will be triggered
    server = require('http').createServer();
    io = require('socket.io')();
    io.attach(server);

    server.listen(port, 'localhost');
    server.on('listening', function () {
        console.log('Electron Socket started on port %s at %s', server.address().port, server.address().address);
    });

    startAspCoreBackend(port);

    io.on('connection', (socket) => {
        global['electronsocket'] = socket;
        global['electronsocket'].setMaxListeners(0);
        console.log('ASP.NET Core Application connected...', 'global.electronsocket', global['electronsocket'].id, new Date());

        appApi = require('./api/app')(socket, app);
        browserWindows = require('./api/browserWindows')(socket, app);
        ipc = require('./api/ipc')(socket);
        menu = require('./api/menu')(socket);
        dialogApi = require('./api/dialog')(socket);
        notification = require('./api/notification')(socket);
        tray = require('./api/tray')(socket);
        webContents = require('./api/webContents')(socket);
        globalShortcut = require('./api/globalShortcut')(socket);
        shellApi = require('./api/shell')(socket);
        screen = require('./api/screen')(socket);
        clipboard = require('./api/clipboard')(socket);

        if (splashScreen && !splashScreen.isDestroyed()) {
            splashScreen.close();
        }

        try {
            const hostHookScriptFilePath = path.join(__dirname, 'bin', 'ElectronHostHook', 'index.js');
            const { HookService } = require(hostHookScriptFilePath);
            if (hostHook === undefined) {
                hostHook = new HookService(socket, app);
                hostHook.onHostReady();
            }
        } catch (error) {
            console.log(error.message);
        }
    });
}

function startAspCoreBackend(electronPort) {

    // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
    portscanner.findAPortNotInUse(8000, 65535, 'localhost', function (error, electronWebPort) {
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
