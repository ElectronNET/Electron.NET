const { app, BrowserWindow, protocol } = require('electron');
const { findAPortNotInUse } = require('portscanner');
const { imageSize } = require('image-size');
const { resolve, join } = require('node:path');
const { spawn } = require('node:child_process');

let io, server, apiProcess, loadURL, splashScreen, hostHook;
let launchFile, launchUrl;

// APIs
let appApi, autoUpdater, browserView, browserWindows, clipboard, commandLine, dialogApi, dock,
    globalShortcut, ipc, menu, nativeTheme, notification, powerMonitor, screen, shellApi, tray,
    webContents

let manifestJsonFileName = 'electron.manifest.json';
let watchable = false;
if (app.commandLine.hasSwitch('manifest')) manifestJsonFileName = app.commandLine.getSwitchValue('manifest');
if (app.commandLine.hasSwitch('watch')) watchable = true;

let currentBinPath = resolve(__dirname.replace('app.asar', ''), 'bin');
let manifestJsonFilePath = join(currentBinPath, manifestJsonFileName);

// if watch is enabled lets change the path
if (watchable) {
    currentBinPath = resolve(__dirname, '../../'); // go to project directory
    manifestJsonFilePath = join(currentBinPath, manifestJsonFileName);
}

//  handle macOS events for opening the app with a file, etc
app.on('will-finish-launching', () => {
    app.on('open-file', (evt, file) => {
        evt.preventDefault();
        launchFile = file;
    });
    app.on('open-url', (evt, url) => {
        evt.preventDefault();
        launchUrl = url;
    });
});

const manifestJsonFile = require(manifestJsonFilePath);
if (manifestJsonFile.singleInstance || manifestJsonFile.aspCoreBackendPort) {
    const mainInstance = app.requestSingleInstanceLock();
    app.on('second-instance', (events, args = []) => {
        args.forEach((parameter) => {
            const words = parameter.split('=');

            if (words.length > 1) {
                app.commandLine.appendSwitch(words[0].replace('--', ''), words[1]);
            } else {
                app.commandLine.appendSwitch(words[0].replace('--', ''));
            }
        });

        const windows = BrowserWindow.getAllWindows();
        if (windows.length) {
            if (windows[0].isMinimized())
                windows[0].restore();
            windows[0].focus();
        }
    });

    if (!mainInstance) app.quit();
}

app.on('ready', () => {
    // Fix ERR_UNKNOWN_URL_SCHEME using file protocol
    // https://github.com/electron/electron/issues/23757
    protocol.registerFileProtocol('file', (request, callback) => {
        const pathname = request.url.replace('file:///', '');
        callback(pathname);
    });

    if (isSplashScreenEnabled()) {
        startSplashScreen();
    }
    // Added default port as configurable for port restricted environments.
    let defaultElectronPort = 8000;
    if (manifestJsonFile.electronPort) {
        defaultElectronPort = manifestJsonFile.electronPort;
    }
    // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
    findAPortNotInUse(defaultElectronPort, 65535, 'localhost', function (error, port) {
        console.log('Electron Socket IO Port: ' + port);
        startSocketApiBridge(port);
    });
});

app.on('quit', async (event, exitCode) => {
    await server.close();
    apiProcess.kill();
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
    let imageFile = join(currentBinPath, manifestJsonFile.splashscreen.imageFile);
    imageSize(imageFile, (error, dimensions) => {
        if (error) {
            console.log(`load splashscreen error:`);
            console.error(error);

            throw new Error(error.message);
        }

        splashScreen = new BrowserWindow({
            width: dimensions.width,
            height: dimensions.height,
            transparent: true,
            center: true,
            frame: false,
            closable: false,
            resizable: false,
            skipTaskbar: true,
            alwaysOnTop: true,
            show: true,
        });
        splashScreen.setIgnoreMouseEvents(true);

        app.once('browser-window-created', () => splashScreen.destroy());

        const loadSplashscreenUrl = join(__dirname, 'splashscreen', 'index.html') + '?imgPath=' + imageFile;
        splashScreen.loadURL('file://' + loadSplashscreenUrl);

        splashScreen.once('closed', () => splashScreen = null);
    });
}

function startSocketApiBridge(port) {
    // instead of 'require('socket.io')(port);' we need to use this workaround
    // otherwise the Windows Firewall will be triggered
    server = require('node:http').createServer();
    const { Server } = require('socket.io');
    io = new Server({
        pingTimeout: 60000, // in ms, default is 5000
        pingInterval: 10000, // in ms, default is 25000
    });
    io.attach(server);

    server.listen(port, 'localhost');
    server.on('listening', function () {
        console.log('Electron Socket started on port %s at %s', server.address().port, server.address().address);
        // Now that socket connection is established, we can guarantee port will not be open for portscanner
        if (watchable) {
            startAspCoreBackendWithWatch(port);
        } else {
            startAspCoreBackend(port);
        }
    });

    // prototype
    app['mainWindowURL'] = '';
    app['mainWindow'] = null;

    io.on('connection', (socket) => {
        socket.on('disconnect', function (reason) {
            console.log('Got disconnect! Reason: ' + reason);
            try {
                if (hostHook) {
                    delete require.cache[require.resolve("@electron-host/hook")];
                    hostHook = undefined;
                }
            } catch (error) {
                console.error(error.message);
            }
        });

        if (global['electronsocket'] === undefined) {
            global['electronsocket'] = socket;
            global['electronsocket'].setMaxListeners(0);
        }

        console.log('ASP.NET Core Application connected...', 'global.electronsocket', global['electronsocket'].id, new Date());

        if (appApi === undefined) appApi = require('@electron-host/api/app')(socket, app);
        if (browserWindows === undefined) browserWindows = require('@electron-host/api/browserWindows')(socket, app);
        if (commandLine === undefined) commandLine = require('@electron-host/api/commandLine')(socket, app);
        if (autoUpdater === undefined) autoUpdater = require('@electron-host/api/autoUpdater')(socket);
        if (ipc === undefined) ipc = require('@electron-host/api/ipc')(socket);
        if (menu === undefined) menu = require('@electron-host/api/menu')(socket);
        if (dialogApi === undefined) dialogApi = require('@electron-host/api/dialog')(socket);
        if (notification === undefined) notification = require('@electron-host/api/notification')(socket);
        if (tray === undefined) tray = require('@electron-host/api/tray')(socket);
        if (webContents === undefined) webContents = require('@electron-host/api/webContents')(socket);
        if (globalShortcut === undefined) globalShortcut = require('@electron-host/api/globalShortcut')(socket);
        if (shellApi === undefined) shellApi = require('@electron-host/api/shell')(socket);
        if (screen === undefined) screen = require('@electron-host/api/screen')(socket);
        if (clipboard === undefined) clipboard = require('@electron-host/api/clipboard')(socket);
        if (browserView === undefined) browserView = require('@electron-host/api/browserView').browserViewApi(socket);
        if (powerMonitor === undefined) powerMonitor = require('@electron-host/api/powerMonitor')(socket);
        if (nativeTheme === undefined) nativeTheme = require('@electron-host/api/nativeTheme')(socket);
        if (dock === undefined) dock = require('@electron-host/api/dock')(socket);

        socket.on('register-app-open-file-event', (id) => {
            global['electronsocket'] = socket;

            app.on('open-file', (event, file) => {
                event.preventDefault();

                global['electronsocket'].emit('app-open-file' + id, file);
            });

            if (launchFile) global['electronsocket'].emit('app-open-file' + id, launchFile);
        });

        socket.on('register-app-open-url-event', (id) => {
            global['electronsocket'] = socket;

            app.on('open-url', (event, url) => {
                event.preventDefault();

                global['electronsocket'].emit('app-open-url' + id, url);
            });

            if (launchUrl) global['electronsocket'].emit('app-open-url' + id, launchUrl);
        });

        try {
            const { HookService } = require("@electron-host/hook");

            if (hostHook === undefined) {
                hostHook = new HookService(socket, app);
                hostHook.onHostReady();
            }
        } catch (error) {
            console.error(error.message);
        }
    });
}

function startAspCoreBackend(electronPort) {
    if (manifestJsonFile.aspCoreBackendPort) {
        startBackend(manifestJsonFile.aspCoreBackendPort);
    } else {
        // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
        findAPortNotInUse(electronPort + 1, 65535, 'localhost', (error, electronWebPort) => {
            startBackend(electronWebPort);
        });
    }

    function startBackend(aspCoreBackendPort) {
        console.log('ASP.NET Core Port: ' + aspCoreBackendPort);
        loadURL = `http://localhost:${aspCoreBackendPort}`;
        const parameters = [
            getEnvironmentParameter(),
            `/electronPort=${electronPort}`,
            `/electronWebPort=${aspCoreBackendPort}`,
        ];

        const { platform } = require('node:os');
        let binaryFile = platform() === 'win32' ? `${manifestJsonFile.executable}.exe` : manifestJsonFile.executable;

        apiProcess = spawn(join(currentBinPath, binaryFile), parameters, {
            cwd: currentBinPath
        });

        apiProcess.stdout.on('data', (data) => {
            console.log(`stdout: ${data.toString()}`);
        });
    }
}

function startAspCoreBackendWithWatch(electronPort) {
    if (manifestJsonFile.aspCoreBackendPort) {
        startBackend(manifestJsonFile.aspCoreBackendPort);
    } else {
        // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
        findAPortNotInUse(electronPort + 1, 65535, 'localhost', function (error, electronWebPort) {
            startBackend(electronWebPort);
        });
    }

    function startBackend(aspCoreBackendPort) {
        console.log('ASP.NET Core Watch Port: ' + aspCoreBackendPort);
        loadURL = `http://localhost:${aspCoreBackendPort}`;
        const parameters = [
            'watch',
            'run',
            getEnvironmentParameter(),
            `/electronPort=${electronPort}`,
            `/electronWebPort=${aspCoreBackendPort}`,
        ];

        apiProcess = spawn('dotnet', parameters, {
            cwd: currentBinPath,
            env: process.env,
        });

        apiProcess.stdout.on('data', (data) => {
            console.log(`stdout: ${data.toString()}`);
        });
    }
}

function getEnvironmentParameter() {
    return manifestJsonFile.environment ? '--environment=' + manifestJsonFile.environment : '';
}
