const { app } = require('electron');
const { BrowserWindow } = require('electron');
const path = require('path');
const process = require('child_process').spawn;
const portscanner = require('portscanner');
const imageSize = require('image-size');
let io, server, browserWindows, ipc, apiProcess, loadURL;
let appApi, menu, dialogApi, notification, tray, webContents;
let globalShortcut, shellApi, screen, clipboard, autoUpdater;
let splashScreen, hostHook;

const currentBinPath = path.join(__dirname.replace('app.asar', ''), 'bin');
const manifestJsonFilePath = path.join(currentBinPath, 'electron.manifest.json');
const manifestJsonFile = require(manifestJsonFilePath);
if (manifestJsonFile.singleInstance || manifestJsonFile.aspCoreBackendPort) {
    const mainInstance = app.requestSingleInstanceLock();
    app.on('second-instance', () => {
        const windows = BrowserWindow.getAllWindows();
        if (windows.length) {
            if (windows[0].isMinimized()) {
                windows[0].restore();
            }
            windows[0].focus();
        }
    });

    if (!mainInstance) {
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
    if (manifestJsonFile.hasOwnProperty('splashscreen')) {
        if (manifestJsonFile.splashscreen.hasOwnProperty('imageFile')) {
            return Boolean(manifestJsonFile.splashscreen.imageFile);
        }
    }

    return false;
}

function startSplashScreen() {
    let imageFile = path.join(currentBinPath, manifestJsonFile.splashscreen.imageFile);
    imageSize(imageFile, (error, dimensions) => {
        if (error) {
            console.log(`load splashscreen error:`);
            console.log(error);

            throw new Error(error.message);
        }

        splashScreen = new BrowserWindow({
            width: dimensions.width,
            height: dimensions.height,
            transparent: true,
            center: true,
            frame: false,
            alwaysOnTop: true,
            skipTaskbar: true,
            show: true
        });

        app.once('browser-window-focus', () => {
            app.once('browser-window-focus', () => {
                splashScreen.destroy();
            });
        });

        const loadSplashscreenUrl = path.join(__dirname, 'splashscreen', 'index.html') + '?imgPath=' + imageFile;
        splashScreen.loadURL('file://' + loadSplashscreenUrl);

        splashScreen.once('closed', () => {
            splashScreen = null;
        });
    });
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
        // Now that socket connection is established, we can guarantee port will not be open for portscanner
        startAspCoreBackend(port);
    });

    io.on('connection', (socket) => {
        global['electronsocket'] = socket;
        global['electronsocket'].setMaxListeners(0);
        console.log('ASP.NET Core Application connected...', 'global.electronsocket', global['electronsocket'].id, new Date());

        appApi = require('./api/app')(socket, app);
        browserWindows = require('./api/browserWindows')(socket, app);
        autoUpdater = require('./api/autoUpdater')(socket);
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

        try {
            const hostHookScriptFilePath = path.join(__dirname, 'ElectronHostHook', 'index.js');

            if (isModuleAvailable(hostHookScriptFilePath) && hostHook === undefined) {
                const { HookService } = require(hostHookScriptFilePath);
                hostHook = new HookService(socket, app);
                hostHook.onHostReady();
            }
        } catch (error) {
            console.log(error.message);
        }
    });
}

function isModuleAvailable(name) {
    try {
        require.resolve(name);
        return true;
    } catch (e) { }
    return false;
}

function startAspCoreBackend(electronPort) {
    if(manifestJsonFile.aspCoreBackendPort) {
        startBackend(manifestJsonFile.aspCoreBackendPort)
    } else {
        // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
        portscanner.findAPortNotInUse(8000, 65535, 'localhost', function (error, electronWebPort) {
            startBackend(electronWebPort);
        });
    }

    function startBackend(aspCoreBackendPort) {
        console.log('ASP.NET Core Port: ' + aspCoreBackendPort);
        loadURL = `http://localhost:${aspCoreBackendPort}`;
        const parameters = [`/electronPort=${electronPort}`, `/electronWebPort=${aspCoreBackendPort}`];
        let binaryFile = manifestJsonFile.executable;

        const os = require('os');
        if (os.platform() === 'win32') {
            binaryFile = binaryFile + '.exe';
        }

        let binFilePath = path.join(currentBinPath, binaryFile);
        var options = { cwd: currentBinPath };
        apiProcess = process(binFilePath, parameters, options);

        apiProcess.stdout.on('data', (data) => {
            console.log(`stdout: ${data.toString()}`);
        });
    }
}
