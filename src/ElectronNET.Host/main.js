const { app } = require('electron');
const { BrowserWindow } = require('electron');
const { protocol } = require('electron');
const path = require('path');
const cProcess = require('child_process').spawn;
const portscanner = require('portscanner');
const { imageSize } = require('image-size');
let io, server, browserWindows, ipc, apiProcess, loadURL;
let appApi, menu, dialogApi, notification, tray, webContents;
let globalShortcut, shellApi, screen, clipboard, autoUpdater;
let commandLine, browserView;
let powerMonitor;
let processInfo;
let splashScreen;
let nativeTheme;
let dock;
let launchFile;
let launchUrl;
let processApi;

let manifestJsonFileName = 'package.json';
let unpackedelectron = false;
let unpackeddotnet = false;
let dotnetpacked = false;
let electronforcedport;

if (app.commandLine.hasSwitch('manifest')) {
    manifestJsonFileName = app.commandLine.getSwitchValue('manifest');
}

console.log('Entry!!!:  ');

if (app.commandLine.hasSwitch('unpackedelectron')) {
    unpackedelectron = true;
}
else if (app.commandLine.hasSwitch('unpackeddotnet')) {
    unpackeddotnet = true;
}
else if (app.commandLine.hasSwitch('dotnetpacked')) {
    dotnetpacked = true;
}

if (app.commandLine.hasSwitch('electronforcedport')) {
    electronforcedport = app.commandLine.getSwitchValue('electronforcedport');
}

// Custom startup hook: look for custom_main.js and invoke its onStartup(host) if present.
// If the hook returns false, abort Electron startup.
try {
    const fs = require('fs');
    const customMainPath = path.join(__dirname, 'custom_main.js');
    if (fs.existsSync(customMainPath)) {
        const customMain = require(customMainPath);
        if (customMain && typeof customMain.onStartup === 'function') {
            const continueStartup = customMain.onStartup(globalThis);
            if (continueStartup === false) {
                ////console.log('custom_main.js onStartup returned false. Exiting Electron host.');
                // Ensure the app terminates immediately before further initialization.
                // Use app.exit to allow Electron to perform its shutdown, fallback to process.exit.
                try { app.exit(0); } catch (err) { process.exit(0); }
            }
        } else {
            console.warn('custom_main.js found but no onStartup function exported.');
        }
    }
} catch (err) {
    console.error('Error while executing custom_main.js:', err);
}

const currentPath = __dirname;
let currentBinPath = path.join(currentPath.replace('app.asar', ''), 'bin');
let manifestJsonFilePath = path.join(currentPath, manifestJsonFileName);

// if running unpackedelectron, lets change the path
if (unpackedelectron || unpackeddotnet) {
    console.log('unpackedelectron! dir: ' + currentPath);

    manifestJsonFilePath = path.join(currentPath, manifestJsonFileName);
    currentBinPath = path.join(currentPath, '../'); // go to project directory
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

if (manifestJsonFile.singleInstance === "yes") {
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

// Collect user supplied command line args (excluding those handled by Electron host itself)
function getForwardedArgs() {
    const skipSwitches = new Set(['unpackedelectron', 'unpackeddotnet', 'dotnetpacked']);
    return process.argv.slice(2).filter(arg => {
        if (!arg) return false;
        // Node/Electron internal or we already process them
        if (arg.startsWith('--manifest')) return false;
        const cleaned = arg.replace(/^--/, '').replace(/^\//, '');
        if (skipSwitches.has(cleaned)) return false;
        if (cleaned.startsWith('inspect')) return false;
        if (cleaned.startsWith('remote-debugging-port')) return false;
        // We add /electronPort ourselves later
        if (cleaned.startsWith('electronPort=')) return false;
        if (cleaned.startsWith('electronWebPort=')) return false;
        return true;
    });
}

const forwardedArgs = getForwardedArgs();

app.on('ready', () => {
    // Fix ERR_UNKNOWN_URL_SCHEME using file protocol
    // https://github.com/electron/electron/issues/23757
    ////protocol.registerFileProtocol('file', (request, callback) => {
    ////  const pathname = request.url.replace('file:///', '');
    ////  callback(pathname);
    ////});

    if (isSplashScreenEnabled()) {
        startSplashScreen();
    }

    if (electronforcedport) {
        console.log('Electron Socket IO (forced) Port: ' + electronforcedport);
        startSocketApiBridge(electronforcedport);
        return;
    }

    // Added default port as configurable for port restricted environments.
    let defaultElectronPort = 8000;
    if (manifestJsonFile.electronPort) {
        defaultElectronPort = manifestJsonFile.electronPort;
    }

    // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
    portscanner.findAPortNotInUse(defaultElectronPort, 65535, 'localhost', function (error, port) {
        console.log('Electron Socket IO Port: ' + port);
        startSocketApiBridge(port);
    });
});

app.on('quit', async (event, exitCode) => {
    try {
        server.close();
        server.closeAllConnections();
    } catch (e) {
        console.error(e);
    }

    try {
        apiProcess?.kill();
    } catch (e) {
        console.error(e);
    }

    try {
        if (io && typeof io.close === 'function') {
            io.close();
        }
    } catch (e) {
        console.error(e);
    }
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
    const imageFile = path.join(currentPath, manifestJsonFile.splashscreen.imageFile);
    const isHtml = imageFile.endsWith('.html') || imageFile.endsWith('.htm');
    const startWindow = (width, height) => {
        splashScreen = new BrowserWindow({
            width: width,
            height: height,
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

        app.once('browser-window-created', () => {
            splashScreen.destroy();
        });

        const loadSplashscreenUrl = isHtml ? imageFile : path.join(currentPath, 'splashscreen', 'index.html') + '?imgPath=' + imageFile;
        splashScreen.loadURL('file://' + loadSplashscreenUrl);
        splashScreen.once('closed', () => {
            splashScreen = null;
        });
    };

    if (manifestJsonFile.splashscreen.width && manifestJsonFile.splashscreen.height) {
        // width and height are set explicitly
        return startWindow(manifestJsonFile.splashscreen.width, manifestJsonFile.splashscreen.height);
    }

    if (isHtml) {
        // we cannot compute width and height => use default
        return startWindow(800, 600);
    }

    // it's an image, so we can compute the desired splash screen size
    imageSize(imageFile, (error, dimensions) => {
        if (error) {
            console.log(`load splashscreen error:`);
            console.error(error);

            throw new Error(error.message);
        }

        startWindow(dimensions.width, dimensions.height);
    });
}

function startSocketApiBridge(port) {
    // instead of 'require('socket.io')(port);' we need to use this workaround
    // otherwise the Windows Firewall will be triggered
    console.log('Electron Socket: starting...');
    server = require('http').createServer();
    const { Server } = require('socket.io');
    let hostHook;
    io = new Server({
        pingTimeout: 60000, // in ms, default is 5000
        pingInterval: 10000, // in ms, default is 25000
    });
    io.attach(server);

    server.listen(port, 'localhost');
    server.on('listening', function () {
        console.log('Electron Socket: listening on port %s at %s', server.address().port, server.address().address);
        // Now that socket connection is established, we can guarantee port will not be open for portscanner
        if (unpackedelectron) {
            startAspCoreBackendUnpackaged(port);
        } else if (!unpackeddotnet && !dotnetpacked) {
            startAspCoreBackend(port);
        }
    });

    // prototype
    app['mainWindowURL'] = '';
    app['mainWindow'] = null;

    // @ts-ignore
    io.on('connection', (socket) => {
        console.log('Electron Socket: connected!');
        socket.on('disconnect', function (reason) {
            console.log('Got disconnect! Reason: ' + reason);
            try {
                ////console.log('requireCache');
                ////console.log(require.cache['electron-host-hook']);

                if (hostHook) {
                    const hostHookScriptFilePath = path.join(currentPath, 'ElectronHostHook', 'index.js');
                    delete require.cache[require.resolve(hostHookScriptFilePath)];
                    hostHook = undefined;
                }
            } catch (err) {
                console.error(err.message);
            }
        });

        if (global['electronsocket'] === undefined) {
            global['electronsocket'] = socket;
            global['electronsocket'].setMaxListeners(0);
        }

        console.log('Electron Socket: loading components...');

        if (appApi === undefined) appApi = require('./api/app')(socket, app);
        if (browserWindows === undefined) browserWindows = require('./api/browserWindows')(socket, app);
        if (commandLine === undefined) commandLine = require('./api/commandLine')(socket, app);
        if (autoUpdater === undefined) autoUpdater = require('./api/autoUpdater')(socket);
        if (ipc === undefined) ipc = require('./api/ipc')(socket);
        if (menu === undefined) menu = require('./api/menu')(socket);
        if (dialogApi === undefined) dialogApi = require('./api/dialog')(socket);
        if (notification === undefined) notification = require('./api/notification')(socket);
        if (tray === undefined) tray = require('./api/tray')(socket);
        if (webContents === undefined) webContents = require('./api/webContents')(socket);
        if (globalShortcut === undefined) globalShortcut = require('./api/globalShortcut')(socket);
        if (shellApi === undefined) shellApi = require('./api/shell')(socket);
        if (screen === undefined) screen = require('./api/screen')(socket);
        if (clipboard === undefined) clipboard = require('./api/clipboard')(socket);
        if (browserView === undefined) browserView = require('./api/browserView').browserViewApi(socket);
        if (powerMonitor === undefined) powerMonitor = require('./api/powerMonitor')(socket);
        if (nativeTheme === undefined) nativeTheme = require('./api/nativeTheme')(socket);
        if (dock === undefined) dock = require('./api/dock')(socket);
        if (processInfo === undefined) processInfo = require('./api/process')(socket);

        socket.on('register-app-open-file', (id) => {
            global['electronsocket'] = socket;

            app.on('open-file', (event, file) => {
                event.preventDefault();

                global['electronsocket'].emit('app-open-file' + id, file);
            });

            if (launchFile) {
                global['electronsocket'].emit('app-open-file' + id, launchFile);
            }
        });

        socket.on('register-app-open-url', (id) => {
            global['electronsocket'] = socket;

            app.on('open-url', (event, url) => {
                event.preventDefault();

                global['electronsocket'].emit('app-open-url' + id, url);
            });

            if (launchUrl) {
                global['electronsocket'].emit('app-open-url' + id, launchUrl);
            }
        });

        try {
            const { HookService } = require('electron-host-hook');

            if (hostHook === undefined) {
                hostHook = new HookService(socket, app);
                hostHook.onHostReady();
            }
        } catch (error) {
            console.error(error.message);
        }

        console.log('Electron Socket: startup complete.');
    });
}

function startAspCoreBackend(electronPort) {
    startBackend();

    function startBackend() {
        loadURL = `about:blank`;
        const envParam = getEnvironmentParameter();
        const parameters = [
            envParam,
            `/electronPort=${electronPort}`,
            `/electronPID=${process.pid}`,
            // forward user supplied args (avoid duplicate environment)
            ...forwardedArgs.filter(a => !(envParam && a.startsWith('--environment=')))
        ].filter(p => p);
        let binaryFile = manifestJsonFile.executable;

        const os = require('os');
        if (os.platform() === 'win32') {
            binaryFile = binaryFile + '.exe';
        }

        let binFilePath = path.join(currentBinPath, binaryFile);
        var options = { cwd: currentBinPath };
        console.log('Starting backend with parameters:', parameters.join(' '));
        apiProcess = cProcess(binFilePath, parameters, options);

        apiProcess.stdout.on('data', (data) => {
            console.log(`stdout: ${data.toString()}`);
        });
    }
}

function startAspCoreBackendUnpackaged(electronPort) {
    startBackend();

    function startBackend() {
        loadURL = `about:blank`;
        const envParam = getEnvironmentParameter();
        const parameters = [
            envParam,
            `/electronPort=${electronPort}`,
            `/electronPID=${process.pid}`,
            ...forwardedArgs.filter(a => !(envParam && a.startsWith('--environment=')))
        ].filter(p => p);
        let binaryFile = manifestJsonFile.executable;

        const os = require('os');
        if (os.platform() === 'win32') {
            binaryFile = binaryFile + '.exe';
        }

        let binFilePath = path.join(currentBinPath, binaryFile);
        var options = { cwd: currentBinPath };
        console.log('Starting backend (unpackaged) with parameters:', parameters.join(' '));
        apiProcess = cProcess(binFilePath, parameters, options);

        apiProcess.stdout.on('data', (data) => {
            console.log(`stdout: ${data.toString()}`);
        });
    }
}

function getEnvironmentParameter() {
    if (manifestJsonFile.environment) {
        return '--environment=' + manifestJsonFile.environment;
    }

    return '';
}
