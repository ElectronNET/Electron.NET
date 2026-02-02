const { app } = require('electron');
const { BrowserWindow } = require('electron');
const { protocol } = require('electron');
const path = require('path');
const cProcess = require('child_process').spawn;
const portscanner = require('portscanner');
const { imageSize } = require('image-size');
const { logger } = require('./logger');

let io, server, browserWindows, ipc, apiProcess, loadURL;
let appApi, menu, dialogApi, notification, tray, webContents;
let globalShortcut, shellApi, screen, clipboard, autoUpdater;
let commandLine, browserView;
let powerMonitor;
let processInfo;
let splashScreen;
let nativeTheme;
let dock;
let desktopCapturer;
let electronHostHook;
let touchBar;
let launchFile;
let launchUrl;
let processApi;

let manifestJsonFileName = 'package.json';
let unpackedelectron = false;
let unpackeddotnet = false;
let dotnetpacked = false;
let unpackeddotnetsignalr = false;
let dotnetpackedsignalr = false;
let electronforcedport;
let electronUrl;

if (app.commandLine.hasSwitch('manifest')) {
    manifestJsonFileName = app.commandLine.getSwitchValue('manifest');
}

// Check for SignalR modes first (these take precedence)
if (app.commandLine.hasSwitch('unpackeddotnetsignalr')) {
    unpackeddotnetsignalr = true;
}
else if (app.commandLine.hasSwitch('dotnetpackedsignalr')) {
    dotnetpackedsignalr = true;
}
// Then check legacy modes
else if (app.commandLine.hasSwitch('unpackedelectron')) {
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

let authToken;
if (app.commandLine.hasSwitch('authtoken')) {
    authToken = app.commandLine.getSwitchValue('authtoken');
    // Store in global for access by browser windows
    global.authToken = authToken;
}

if (app.commandLine.hasSwitch('electronurl')) {
    electronUrl = app.commandLine.getSwitchValue('electronurl');
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
            logger.warn('custom_main.js found but no onStartup function exported.');
        }
    }
} catch (err) {
    logger.error('Error while executing custom_main.js:', err);
}

const currentPath = __dirname;
let currentBinPath = path.join(currentPath.replace('app.asar', ''), 'bin');
let manifestJsonFilePath = path.join(currentPath, manifestJsonFileName);

// if running unpackedelectron, lets change the path
if (unpackedelectron || unpackeddotnet) {
    logger.debug('Running in unpacked mode, dir: ' + currentPath);

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

if (manifestJsonFile.singleInstance) {
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

app.on('ready', async () => {
    // Start overall startup timer
    logger.time('[Startup] Total Electron Startup');
    
    // Fix ERR_UNKNOWN_URL_SCHEME using file protocol
    // https://github.com/electron/electron/issues/23757
    ////protocol.registerFileProtocol('file', (request, callback) => {
    ////  const pathname = request.url.replace('file:///', '');
    ////  callback(pathname);
    ////});

    if (isSplashScreenEnabled()) {
        startSplashScreen();
    }

    // Check if we're using SignalR-based startup
    // SignalR mode is activated by --unpackeddotnetsignalr or --dotnetpackedsignalr flags
    // .NET passes the actual server URL via --electronurl parameter (no port scanning needed)
    if (unpackeddotnetsignalr || dotnetpackedsignalr) {
        if (!electronUrl) {
            logger.error('[Electron] ERROR: SignalR mode requires --electronUrl parameter');
            app.quit();
            return;
        }
        
        // Create a temporary invisible window to keep Electron alive during startup.
        // Without any windows, Electron would quit immediately on macOS.
        // This will be destroyed once the first real window is created.
        const { BrowserWindow } = require('electron');
        const keepAliveWindow = new BrowserWindow({
            show: false,
            width: 1,
            height: 1
        });
        
        // Destroy the keep-alive window when the first real window is created
        app.once('browser-window-created', (event, window) => {
            if (keepAliveWindow && !keepAliveWindow.isDestroyed()) {
                keepAliveWindow.destroy();
            }
        });
        
        await startSignalRApiBridge(electronUrl);
        logger.timeEnd('[Startup] Total Electron Startup');
        return;
    }

    // Legacy socket.io startup
    if (electronforcedport) {
        logger.info('Electron Socket IO (forced) Port: ' + electronforcedport);
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
        logger.info('Electron Socket IO Port: ' + port);
        startSocketApiBridge(port);
    });
});

app.on('quit', async (event, exitCode) => {
    // Clean up Socket.IO resources (legacy mode only)
    if (typeof server !== 'undefined' && server) {
        try {
            server.close();
            server.closeAllConnections();
        } catch (e) {
            logger.error('Error closing Socket.IO server:', e);
        }
    }

    // Clean up API process (Socket.IO mode only)
    if (typeof apiProcess !== 'undefined' && apiProcess) {
        try {
            apiProcess.kill();
        } catch (e) {
            logger.error('Error killing API process:', e);
        }
    }

    // Clean up Socket.IO connection (legacy mode only)
    if (typeof io !== 'undefined' && io && typeof io.close === 'function') {
        try {
            io.close();
        } catch (e) {
            logger.error('Error closing Socket.IO connection:', e);
        }
    }
    
    // Clean up SignalR connection (SignalR mode only)
    if (global['electronsignalr'] && typeof global['electronsignalr'].connection !== 'undefined') {
        try {
            await global['electronsignalr'].connection.stop();
        } catch (e) {
            logger.error('Error closing SignalR connection:', e);
        }
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
            logger.error(`load splashscreen error:`, error);
            throw new Error(error.message);
        }

        startWindow(dimensions.width, dimensions.height);
    });
}

function startSocketApiBridge(port) {
    // instead of 'require('socket.io')(port);' we need to use this workaround
    // otherwise the Windows Firewall will be triggered
    logger.debug('Electron Socket: starting...');
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
        logger.info('Electron Socket: listening on port %s at %s', server.address().port, server.address().address);
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
        logger.info('Electron Socket: connected!');
        socket.on('disconnect', function (reason) {
            logger.debug('Got disconnect! Reason: ' + reason);
            try {
                ////console.log('requireCache');
                ////console.log(require.cache['electron-host-hook']);

                if (hostHook) {
                    const hostHookScriptFilePath = path.join(currentPath, 'ElectronHostHook', 'index.js');
                    delete require.cache[require.resolve(hostHookScriptFilePath)];
                    hostHook = undefined;
                }
            } catch (err) {
                logger.error(err.message);
            }
        });

        if (global['electronsocket'] === undefined) {
            global['electronsocket'] = socket;
            global['electronsocket'].setMaxListeners(0);
        }

        logger.debug('Electron Socket: loading components...');

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
            logger.error(error.message);
        }

        logger.info('Electron Socket: startup complete.');
    });
}

/**
 * Starts the SignalR API bridge for .NET-first SignalR mode.
 * 
 * Flow:
 * 1. Connect to SignalR hub at /electron-hub endpoint
 * 2. Register as Electron client
 * 3. Load all API modules (same modules as Socket.IO mode)
 * 4. Signal 'electron-host-ready' to .NET to trigger app ready callback
 * 
 * This ensures .NET doesn't call the app ready callback until all API modules
 * are loaded and ready to handle requests from .NET code.
 */
async function startSignalRApiBridge(baseUrl) {
    const { SignalRBridge } = require('./api/signalr-bridge');
    const hubUrl = `${baseUrl}/electron-hub`;
    
    // Pass the authentication token to the SignalR bridge
    const signalRBridge = new SignalRBridge(hubUrl, global.authToken);
    
    try {
        logger.time('[Startup] SignalR Connection');
        const connected = await signalRBridge.connect();
        logger.timeEnd('[Startup] SignalR Connection');
        
        if (!connected) {
            logger.error('[SignalRBridge] Failed to connect to SignalR hub');
            app.quit();
            return;
        }
        
        // Store the bridge globally for API access
        global['electronsignalr'] = signalRBridge;
        
        // Load API modules in parallel for faster startup
        logger.time('[Startup] Module Loading');
        
        // Define module loaders - each returns the initialized module
        const loadModules = () => {
            const modules = {};
            
            // Load all modules in parallel using Promise.all
            return Promise.all([
                // Critical modules (always needed)
                Promise.resolve().then(() => modules.appApi = require('./api/app')(signalRBridge, app)),
                Promise.resolve().then(() => modules.browserWindows = require('./api/browserWindows')(signalRBridge, app)),
                Promise.resolve().then(() => modules.commandLine = require('./api/commandLine')(signalRBridge, app)),
                Promise.resolve().then(() => modules.webContents = require('./api/webContents')(signalRBridge)),
                Promise.resolve().then(() => modules.ipc = require('./api/ipc')(signalRBridge)),
                Promise.resolve().then(() => modules.menu = require('./api/menu')(signalRBridge)),
                
                // Secondary modules (commonly used)
                Promise.resolve().then(() => modules.dialogApi = require('./api/dialog')(signalRBridge)),
                Promise.resolve().then(() => modules.notification = require('./api/notification')(signalRBridge)),
                Promise.resolve().then(() => modules.shellApi = require('./api/shell')(signalRBridge)),
                Promise.resolve().then(() => modules.clipboard = require('./api/clipboard')(signalRBridge)),
                Promise.resolve().then(() => modules.screen = require('./api/screen')(signalRBridge)),
                
                // Utility modules (less frequently used)
                Promise.resolve().then(() => modules.autoUpdater = require('./api/autoUpdater')(signalRBridge)),
                Promise.resolve().then(() => modules.tray = require('./api/tray')(signalRBridge)),
                Promise.resolve().then(() => modules.globalShortcut = require('./api/globalShortcut')(signalRBridge)),
                Promise.resolve().then(() => modules.nativeTheme = require('./api/nativeTheme')(signalRBridge)),
                Promise.resolve().then(() => modules.powerMonitor = require('./api/powerMonitor')(signalRBridge)),
                Promise.resolve().then(() => modules.processApi = require('./api/process')(signalRBridge)),
                
                // Platform-specific modules
                Promise.resolve().then(() => {
                    if (process.platform === 'darwin') {
                        modules.dock = require('./api/dock')(signalRBridge, app);
                    }
                })
            ]).then(() => modules);
        };
        
        const modules = await loadModules();
        
        // Assign to global variables (for backward compatibility)
        if (appApi === undefined) appApi = modules.appApi;
        if (browserWindows === undefined) browserWindows = modules.browserWindows;
        if (commandLine === undefined) commandLine = modules.commandLine;
        if (autoUpdater === undefined) autoUpdater = modules.autoUpdater;
        if (ipc === undefined) ipc = modules.ipc;
        if (menu === undefined) menu = modules.menu;
        if (dialogApi === undefined) dialogApi = modules.dialogApi;
        if (notification === undefined) notification = modules.notification;
        if (tray === undefined) tray = modules.tray;
        if (webContents === undefined) webContents = modules.webContents;
        if (globalShortcut === undefined) globalShortcut = modules.globalShortcut;
        if (clipboard === undefined) clipboard = modules.clipboard;
        if (screen === undefined) screen = modules.screen;
        if (shellApi === undefined) shellApi = modules.shellApi;
        if (nativeTheme === undefined) nativeTheme = modules.nativeTheme;
        if (powerMonitor === undefined) powerMonitor = modules.powerMonitor;
        if (dock === undefined && modules.dock) dock = modules.dock;
        if (processApi === undefined) processApi = modules.processApi;
        
        logger.timeEnd('[Startup] Module Loading');
        
        // Signal to .NET that Electron is fully ready (API modules loaded)
        logger.time('[Startup] Host Ready Signal');
        await signalRBridge.emit('electron-host-ready');
        logger.timeEnd('[Startup] Host Ready Signal');
        
    } catch (error) {
        logger.error('[SignalRBridge] Error during startup:', error);
        logger.error('[SignalRBridge] Stack:', error.stack);
        app.quit();
    }
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
        logger.debug('Starting backend with parameters:', parameters.join(' '));
        apiProcess = cProcess(binFilePath, parameters, options);

        apiProcess.stdout.on('data', (data) => {
            logger.debug(`stdout: ${data.toString()}`);
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
        logger.debug('Starting backend (unpackaged) with parameters:', parameters.join(' '));
        apiProcess = cProcess(binFilePath, parameters, options);

        apiProcess.stdout.on('data', (data) => {
            logger.debug(`stdout: ${data.toString()}`);
        });
    }
}

function getEnvironmentParameter() {
    if (manifestJsonFile.environment) {
        return '--environment=' + manifestJsonFile.environment;
    }

    return '';
}
