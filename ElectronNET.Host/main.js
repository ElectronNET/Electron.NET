﻿const { app } = require('electron');
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
let splashScreen, hostHook;
let mainWindowId, nativeTheme;
let dock;
let launchFile;
let launchUrl;
let processApi;

let manifestJsonFileName = 'electron.manifest.json';
let watchable = false;

if (app.commandLine.hasSwitch('manifest')) {
    manifestJsonFileName = app.commandLine.getSwitchValue('manifest');
};

if (app.commandLine.hasSwitch('watch')) {
    watchable = true;
};

let currentBinPath = path.join(__dirname.replace('app.asar', ''), 'bin');
let manifestJsonFilePath = path.join(currentBinPath, manifestJsonFileName);

// if watch is enabled lets change the path
if (watchable) {
    currentBinPath = path.join(__dirname, '../../'); // go to project directory
    manifestJsonFilePath = path.join(currentBinPath, manifestJsonFileName);
}

//  handle macOS events for opening the app with a file, etc
app.on('will-finish-launching', () => {
    app.on('open-file', (evt, file) => {
        evt.preventDefault();
        launchFile = file;
    })
    app.on('open-url', (evt, url) => {
        evt.preventDefault();
        launchUrl = url;
    })
});

const manifestJsonFile = require(manifestJsonFilePath);
if (manifestJsonFile.singleInstance || manifestJsonFile.aspCoreBackendPort) {
    const mainInstance = app.requestSingleInstanceLock();
    app.on('second-instance', (events, args = []) => {
        args.forEach(parameter => {
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

// Bypass all SSL/TLS certificate errors. -- Less secure.
if (manifestJsonFile.ignoreAllCertificateErrors) {
    console.log('All SSL/TLS Certificate errors will be ignored.');
    app.commandLine.appendSwitch('ignore-certificate-errors');
}

// Bypass SSL/TLS certificate errors only for the domain names specified in the electron.manifest.json file.
if (manifestJsonFile.hasOwnProperty('domainNamesToIgnoreCertificateErrors')) {
    if (manifestJsonFile.domainNamesToIgnoreCertificateErrors.length > 0) {
        console.log(`SSL/TLS certificate errors will be ignored for ${manifestJsonFile.domainNamesToIgnoreCertificateErrors.join(', ')}`);

        app.on('certificate-error', (event, webContents, url, error, certificate, callback) => {
            if (shouldIgnoreCertificateForUrl(url)) {
                console.log('SSL/TLS certificate error ignored for URL: ' + url);
                event.preventDefault()
                callback(true)
            }
        })
    }
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
        defaultElectronPort = (manifestJsonFile.electronPort)
    }
    // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
    portscanner.findAPortNotInUse(defaultElectronPort, 65535, 'localhost', function (error, port) {
        console.log('Electron Socket IO Port: ' + port);
        startSocketApiBridge(port);
    });
});

app.on('quit', async (event, exitCode) => {
    await server.close();

    var detachedProcess = false;

    if (manifestJsonFile.hasOwnProperty('detachedProcess')) {
        detachedProcess = manifestJsonFile.detachedProcess;
    }

    if (!detachedProcess) {
        apiProcess.kill();
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
    let imageFile = path.join(currentBinPath, manifestJsonFile.splashscreen.imageFile);
    imageSize(imageFile, (error, dimensions) => {
        if (error) {
            console.log('load splashscreen error:');
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
            show: true
        });

        if (manifestJsonFile.hasOwnProperty('splashscreen')) {
            if (manifestJsonFile.splashscreen.hasOwnProperty('timeout')) {
                var timeout = manifestJsonFile.splashscreen.timeout;
                setTimeout((t) => {
                    if (splashScreen) {
                        splashScreen.hide();
                    }
                }, timeout);
            }
        }


        splashScreen.setIgnoreMouseEvents(true);

        app.once('browser-window-created', () => {
            splashScreen.destroy();
            splashScreen = null;
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
    const { Server } = require('socket.io');
    io = new Server(server);

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
    app['mainWindowURL'] = "";
    app['mainWindow'] = null;

    // @ts-ignore
    io.on('connection', (socket) => {

        socket.on('disconnect', function (reason) {
            console.log('Got disconnect! Reason: ' + reason);
            try {
                if (hostHook) {
                    const hostHookScriptFilePath = path.join(__dirname, 'ElectronHostHook', 'index.js');
                    delete require.cache[require.resolve(hostHookScriptFilePath)];
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
        if (processApi === undefined) processApi = require('./api/process')(socket);

        socket.on('register-app-open-file-event', (id) => {
            global['electronsocket'] = socket;

            app.on('open-file', (event, file) => {
                event.preventDefault();

                global['electronsocket'].emit('app-open-file' + id, file);
            });

            if (launchFile) {
                global['electronsocket'].emit('app-open-file' + id, launchFile);
            }
        });

        socket.on('register-app-open-url-event', (id) => {
            global['electronsocket'] = socket;

            app.on('open-url', (event, url) => {
                event.preventDefault();

                global['electronsocket'].emit('app-open-url' + id, url);
            });

            if (launchUrl) {
                global['electronsocket'].emit('app-open-url' + id, launchUrl);
            }
        });

        socket.on('console-stdout', (data) => {
            console.log(`stdout: ${data.toString()}`);
        });

        socket.on('console-stderr', (data) => {
            console.log(`stderr: ${data.toString()}`);
        });

        try {
            const hostHookScriptFilePath = path.join(__dirname, 'ElectronHostHook', 'index.js');

            if (isModuleAvailable(hostHookScriptFilePath) && hostHook === undefined) {
                const { HookService } = require(hostHookScriptFilePath);
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
    } catch (e) { }
    return false;
}

function startAspCoreBackend(electronPort) {
    if (manifestJsonFile.aspCoreBackendPort) {
        startBackend(manifestJsonFile.aspCoreBackendPort)
    } else {
        // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
        portscanner.findAPortNotInUse(electronPort + 1, 65535, 'localhost', function (error, electronWebPort) {
            startBackend(electronWebPort);
        });
    }

    function startBackend(aspCoreBackendPort) {
        console.log('ASP.NET Core Port: ' + aspCoreBackendPort);
        loadURL = `http://localhost:${aspCoreBackendPort}`;
        const parameters = [getEnvironmentParameter(), `/electronPort=${electronPort}`, `/electronWebPort=${aspCoreBackendPort}`, `/electronPID=${process.pid}`];
        let binaryFile = manifestJsonFile.executable;

        const os = require('os');
        if (os.platform() === 'win32') {
            binaryFile = binaryFile + '.exe';
        }

        var detachedProcess = false;
        var stdioopt = 'pipe';

        if (manifestJsonFile.hasOwnProperty('detachedProcess')) {
            detachedProcess = manifestJsonFile.detachedProcess;
            if (detachedProcess) {
                stdioopt = 'ignore';
            }
        }

        let binFilePath = path.join(currentBinPath, binaryFile);

        var options = { cwd: currentBinPath, detached: detachedProcess, stdio: stdioopt  };

        apiProcess = cProcess(binFilePath, parameters, options);

        if (!detachedProcess) {
            apiProcess.stdout.on('data', (data) => {
                console.log(`stdout: ${data.toString()}`);
            });

            apiProcess.stderr.on('data', (data) => {
                console.log(`stderr: ${data.toString()}`);
            });
        }

        apiProcess.on('close', (code) => {
            console.log(`ASP.NET Process exited with code ${code}`);
            if (code != 0) {
                console.log(`Will quit Electron, as exit code != 0 (got ${code})`);
                app.exit(code);
            }
        });

        if (detachedProcess) {
            console.log('Detached from ASP.NET process');
            apiProcess.unref();

            apiProcess.stderr.on('data', (data) => {
                console.log(`stderr: ${data.toString()}`);
            });
        }
    }
}

function startAspCoreBackendWithWatch(electronPort) {
    if (manifestJsonFile.aspCoreBackendPort) {
        startBackend(manifestJsonFile.aspCoreBackendPort)
    } else {
        // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
        portscanner.findAPortNotInUse(electronPort + 1, 65535, 'localhost', function (error, electronWebPort) {
            startBackend(electronWebPort);
        });
    }

    function startBackend(aspCoreBackendPort) {
        console.log('ASP.NET Core Watch Port: ' + aspCoreBackendPort);
        loadURL = `http://localhost:${aspCoreBackendPort}`;
        const parameters = ['watch', 'run', getEnvironmentParameter(), `/electronPort=${electronPort}`, `/electronWebPort=${aspCoreBackendPort}`, `/electronPID=${process.pid}`];

        var detachedProcess = false;
        var stdioopt = 'pipe';

        if (manifestJsonFile.hasOwnProperty('detachedProcess')) {
            detachedProcess = manifestJsonFile.detachedProcess;
            if (detachedProcess) {
                stdioopt = 'ignore';
            }
        }

        var options = { cwd: currentBinPath, env: process.env, detached: detachedProcess, stdio: stdioopt };

        apiProcess = cProcess('dotnet', parameters, options);

        if (!detachedProcess) {
            apiProcess.stdout.on('data', (data) => {
                console.log(`stdout: ${data.toString()}`);
            });

            apiProcess.stderr.on('data', (data) => {
                console.log(`stderr: ${data.toString()}`);
            });
        }

        apiProcess.on('close', (code) => {
            console.log(`ASP.NET Process exited with code ${code}`);
            if (code != 0) {
                console.log(`Will quit Electron, as exit code != 0 (got ${code})`);
                app.exit(code);
            }
        });

        if (detachedProcess) {
            console.log('Detached from ASP.NET process');
            apiProcess.unref();
        }
      
        apiProcess.stderr.on('data', (data) => {
            console.log(`stderr: ${data.toString()}`);
        });
    }
}

function getEnvironmentParameter() {
    if (manifestJsonFile.environment) {
        return '--environment=' + manifestJsonFile.environment;
    }

    return '';
}

function shouldIgnoreCertificateForUrl(url) {
    if (manifestJsonFile.hasOwnProperty('domainNamesToIgnoreCertificateErrors')) {
        // Removing the scheme from the url so it will cover https and wss://
        const urlWithoutScheme = url.replace(/(^\w+:|^)\/\//, '');
        const sites = manifestJsonFile.domainNamesToIgnoreCertificateErrors.filter((oneSite) => urlWithoutScheme.startsWith(oneSite));

        return sites.length > 0;
    }

    return false;
}
