const { app, BrowserWindow, protocol } = require('electron');
const { findAPortNotInUse } = require('portscanner');
const { imageSize } = require('image-size');
const { resolve, join } = require('node:path');
const { spawn } = require('node:child_process');

let io, server, apiProcess, loadURL, splashScreen, hostHook;
let launchFile, launchUrl;

// APIs
let appApi, autoUpdaterApi, browserViewApi, browserWindowsApi, clipboardApi, commandLineApi,
    dialogApi, dockApi, globalShortcutApi, ipcApi, menuApi, nativeThemeApi, notificationApi,
    powerMonitorApi, screenApi, shellApi, trayApi, webContentsApi

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

        splashScreen.loadFile(join(__dirname, 'splashscreen', 'index.html'), {
            search: `imgPath=${imageFile}`
        });

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
        const hostHookModulePath = join(__dirname, 'ElectronHostHook'),
              apiModulePath = join(__dirname, 'ElectronHostAPI');

        socket.on('disconnect', function (reason) {
            console.log('Got disconnect! Reason: ' + reason);
            try {
                if (hostHook) {
                    delete require.cache[require.resolve(hostHookModulePath)];
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

        try {
            if (isModuleAvailable(apiModulePath)) {
                const API = require(apiModulePath);

                if (appApi === undefined) appApi = API.appApi(socket, app);
                if (browserWindowsApi === undefined) browserWindowsApi = API.browserWindowApi(socket, app);
                if (commandLineApi === undefined) commandLineApi = API.commandLineApi(socket, app);
                if (autoUpdaterApi === undefined) autoUpdaterApi = API.autoUpdaterApi(socket);
                if (ipcApi === undefined) ipcApi = API.ipcApi(socket);
                if (menuApi === undefined) menuApi = API.menuApi(socket);
                if (dialogApi === undefined) dialogApi = API.dialogApi(socket);
                if (notificationApi === undefined) notificationApi = API.notificationApi(socket);
                if (trayApi === undefined) trayApi = API.trayApi(socket);
                if (webContentsApi === undefined) webContentsApi = API.webContentsApi(socket);
                if (globalShortcutApi === undefined) globalShortcutApi = API.globalShortcutApi(socket);
                if (shellApi === undefined) shellApi = API.shellApi(socket);
                if (screenApi === undefined) screenApi = API.screenApi(socket);
                if (clipboardApi === undefined) clipboardApi = API.clipboardApi(socket);
                if (browserViewApi === undefined) browserViewApi = API.browserViewApi(socket);
                if (powerMonitorApi === undefined) powerMonitorApi = API.powerMonitorApi(socket);
                if (nativeThemeApi === undefined) nativeThemeApi = API.nativeThemeApi(socket);
                if (dockApi === undefined) dockApi = API.dockApi(socket);
            }
        } catch (error) {
            console.error(error.message);
        }

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
            if (isModuleAvailable(hostHookModulePath) && hostHook === undefined) {
                const { HookService } = require(hostHookModulePath);
                hostHook = new HookService(socket, app);
                hostHook.onHostReady();
            }
        } catch (error) {
            console.error(error.message);
        }
    });
}

function isModuleAvailable(name) {
    try {
        require.resolve(name);
        return true;
    } catch (e) {}
    return false;
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
