const { app, nativeTheme, BrowserWindow, protocol } = require('electron');
const path = require('path');
const cProcess = require('child_process');
const process = require('process');
const portscanner = require('portscanner');
const { imageSize } = require('image-size');
const { connect } = require('http2');
const crypto = require('crypto');

fixPath(); //For macOS and Linux packaged-apps, the path variable might be missing

let auth = crypto.randomBytes(32).toString('hex');

let io, server, browserWindows, ipc, apiProcess, loadURL;
let appApi, menu, dialogApi, notification, tray, webContents;
let globalShortcut, shellApi, screen, clipboard, autoUpdater;
let commandLine, browserView, desktopCapturer;
let powerMonitor;
let splashScreen, hostHook;
let mainWindowId, nativeThemeApi;
let dock;
let launchFile;
let launchUrl;
let ignoreApiProcessClosed = false;

let manifestJsonFileName = 'electron.manifest.json';
let watchable = false;
let development = false;

if (app.commandLine.hasSwitch('manifest')) {
    manifestJsonFileName = app.commandLine.getSwitchValue('manifest');
};

if (app.commandLine.hasSwitch('watch')) {
    watchable = true;
};

if (app.commandLine.hasSwitch('development')) {
    development = true;
};

let currentBinPath = path.join(__dirname.replace('app.asar', ''), 'bin');
let manifestJsonFilePath = path.join(currentBinPath, manifestJsonFileName);

// if watch is enabled lets change the path
if (watchable) {
    currentBinPath = path.join(__dirname, '../../'); // go to project directory
    manifestJsonFilePath = path.join(currentBinPath, manifestJsonFileName);
}

if (development) {
    auth = app.commandLine.getSwitchValue("devauth");
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

function prepareForUpdate() {
    try { console.log('closing all windows before update'); } catch { }

    ignoreApiProcessClosed = true;

    app.removeAllListeners("window-all-closed");

    const windows = BrowserWindow.getAllWindows();

    if (windows.length) {
        windows.forEach(w => {
            try {
                w.removeAllListeners("close");
                w.hide();
                w.destroy();
            }
            catch {
                //ignore, probably already destroyed
            }
        });
    }
}

app.on('before-quit-for-update', () => { prepareForUpdate(); });

const manifestJsonFile = require(manifestJsonFilePath);

if (manifestJsonFile.singleInstance || manifestJsonFile.aspCoreBackendPort) {
    const mainInstance = app.requestSingleInstanceLock();
    app.on('second-instance', (events, args = []) => {

        let socket = global['electronsocket'];

        if (socket) {
            socket.emit('app-activate-from-second-instance', args);
        }

        //args.forEach(parameter => {
        //    const words = parameter.split('=');

        //    if(words.length > 1) {
        //        app.commandLine.appendSwitch(words[0].replace('--', ''), words[1]);
        //    } else {
        //        app.commandLine.appendSwitch(words[0].replace('--', ''));
        //    }
        //});

        //const windows = BrowserWindow.getAllWindows();
        //if (windows.length) {
        //    if (windows[0].isMinimized()) {
        //        windows[0].restore();
        //    }
        //    windows[0].focus();
        //}
    });

    if (!mainInstance) {
        app.quit();
        return;
    }
}

//Some flags need to be set before app is ready
if (manifestJsonFile.hasOwnProperty('cliFlags') && manifestJsonFile.cliFlags.length > 0) {
    manifestJsonFile.cliFlags.forEach(flag => {
        app.commandLine.appendSwitch(flag);
    });
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

    if (development) {
        let port = parseInt(app.commandLine.getSwitchValue('devport'));
        try { console.log('Electron Socket IO Port: ' + port); } catch { }
        startSocketApiBridge(port);
    } else {

        // Added default port as configurable for port restricted environments.
        let defaultElectronPort = 8000;
        if (manifestJsonFile.electronPort) {
            defaultElectronPort = (manifestJsonFile.electronPort)
            if (defaultElectronPort == 'random') {
                defaultElectronPort = (Math.floor(Math.random() * 2000 + 8000)); //Use random port to reduce risk of race conditions between when we find a free port here, and when the app locks on the port
            }
        }
        // hostname needs to be localhost, otherwise Windows Firewall will be triggered.
        portscanner.findAPortNotInUse(defaultElectronPort, 65535, 'localhost', function (error, port) {
            try { console.log('Electron Socket IO Port: ' + port); } catch { }
            startSocketApiBridge(port);
        });
    }
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

    if (manifestJsonFile.splashscreen.imageFileDark && nativeTheme.shouldUseDarkColors) {
        imageFile = path.join(currentBinPath, manifestJsonFile.splashscreen.imageFileDark);
    }

    imageSize(imageFile, (error, dimensions) => {
        if (error) {
            try {
                console.log('load splashscreen error:');
                console.error(error);
            } catch { }

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

        //Removed as we want to be able to drag the splash screen: splashScreen.setIgnoreMouseEvents(true);

        app.once('browser-window-created', () => {
            if (splashScreen) {
                splashScreen.hide();
            }
            //We cannot destroy the window here as this triggers an electron freeze bug (https://github.com/electron/electron/issues/29050)
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

    let socketBufferSize = 1e8;
    let pingTimeout = 10000;
    let pingInterval = 5000;

    if (manifestJsonFile.hasOwnProperty('socket')) {

        if (manifestJsonFile.socket.hasOwnProperty('socketBufferSize') && manifestJsonFile.socket.bufferSize > 0) {
            socketBufferSize = manifestJsonFile.bufferSize * 1e8;
        }

        if (manifestJsonFile.socket.hasOwnProperty('socketPingTimeout') && manifestJsonFile.socket.pingTimeout > 0) {
            pingTimeout = manifestJsonFile.pingTimeout * 1000;
        }

        if (manifestJsonFile.socket.hasOwnProperty('socketPingInterval') && manifestJsonFile.socket.pingInterval > 0) {
            pingInterval = manifestJsonFile.pingInterval * 1000;
        }
    }

    io.attach(server, { pingTimeout: pingTimeout, pingInterval: pingInterval, maxHttpBufferSize: socketBufferSize });

    server.listen(port, 'localhost');

    server.on('listening', function () {
        try { console.log('Electron Socket started on port %s at %s', server.address().port, server.address().address); } catch { }
        // Now that socket connection is established, we can guarantee port will not be open for portscanner

        if (watchable) {
            startAspCoreBackendWithWatch(port);
        } else if (development) {
            //Nothing else to do
        } else {
            startAspCoreBackend(port);
        }
    });

    // prototype
    app['mainWindowURL'] = "";
    app['mainWindow'] = null;

    let isConnected = false;
    let checkReconnectTimeout = 0;
    // @ts-ignore
    io.on('connection', (socket) => {
        try { console.log('Socket ' + socket.id + ' connected from .NET'); } catch { }
        
        isConnected = true;
        clearTimeout(checkReconnectTimeout);

        socket.on('disconnect', function (reason) {
            try { console.log('Socket ' + socket.id + ' disconnected from .NET with reason: ' + reason); } catch { }
            try {
                if (hostHook) {
                    const hostHookScriptFilePath = path.join(__dirname, 'ElectronHostHook', 'index.js');
                    delete require.cache[require.resolve(hostHookScriptFilePath)];
                    hostHook = undefined;
                }
            } catch (error) {
                try { console.error(error.message); } catch { }
            }

            clearTimeout(checkReconnectTimeout);

            //Give the server 60 seconds to reconnect, otherwise exits
            checkReconnectTimeout = setTimeout((_) => {
                if (!isConnected) {
                    app.exit(57005); //0xDEAD
                }
            }, 60000);

        });

        socket.on("auth", function (authKey) {
            if (authKey != auth) {
                throw new Error("Invalid auth key");
            }

            //We only hook to events on app on the first initialization of each component
            let firstTime = (global['electronsocket'] == undefined);

            global['electronsocket'] = socket;
            socket.setMaxListeners(0);

            try { console.log('.NET connected on socket ' + socket.id + ' on ' + new Date()); } catch { }

            appApi = require('./api/app')(socket, app, firstTime);
            browserWindows = require('./api/browserWindows')(socket, app, firstTime);
            commandLine = require('./api/commandLine')(socket, app);
            autoUpdater = require('./api/autoUpdater')(socket, app);
            ipc = require('./api/ipc')(socket);
            menu = require('./api/menu')(socket);
            dialogApi = require('./api/dialog')(socket);
            notification = require('./api/notification')(socket);
            tray = require('./api/tray')(socket);
            webContents = require('./api/webContents')(socket);
            globalShortcut = require('./api/globalShortcut')(socket);
            shellApi = require('./api/shell')(socket);
            screen = require('./api/screen')(socket);
            desktopCapturer = require('./api/desktopCapturer')(socket);
            clipboard = require('./api/clipboard')(socket);
            browserView = require('./api/browserView').browserViewApi(socket);
            powerMonitor = require('./api/powerMonitor')(socket);
            nativeThemeApi = require('./api/nativeTheme')(socket);
            dock = require('./api/dock')(socket);

            socket.on('splashscreen-destroy', () => {
                if (splashScreen) {
                    splashScreen.destroy();
                    splashScreen = null;
                }
            });

            socket.on('prepare-for-update', () => { prepareForUpdate(); });

            socket.on('register-app-open-file-event', (id) => {
                global['electronsocket'] = socket;

                app.on('open-file', (event, file) => {
                    event.preventDefault();
                    global['electronsocket'].emit('app-open-file' + id, file);
                });

                if (launchFile) {
                    socket.emit('app-open-file' + id, launchFile);
                }
            });

            socket.on('register-app-open-url-event', (id) => {
                global['electronsocket'] = socket;

                app.on('open-url', (event, url) => {
                    event.preventDefault();
                    global['electronsocket'].emit('app-open-url' + id, url);
                });

                if (launchUrl) {
                    socket.emit('app-open-url' + id, launchUrl);
                }
            });

            socket.on('console-stdout', (data) => {
                try { console.log(`stdout: ${data.toString()}`); } catch { }
            });

            socket.on('console-stderr', (data) => {
                try { console.log(`stderr: ${data.toString()}`); } catch { }
            });

            try {
                const hostHookScriptFilePath = path.join(__dirname, 'ElectronHostHook', 'index.js');

                if (isModuleAvailable(hostHookScriptFilePath) && hostHook === undefined) {
                    const { HookService } = require(hostHookScriptFilePath);
                    hostHook = new HookService(socket, app);
                    hostHook.onHostReady();
                }
            } catch (error) {
                try { console.error(error.message); } catch { }
            }
        });
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
        console.log('.NET Core Port: ' + aspCoreBackendPort);
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
                stdioopt = ['pipe', 'ignore', 'ignore'];
            }
        }

        let env = process.env;

        if (manifestJsonFile.hasOwnProperty('variables')) {
            env = { ...env, ...manifestJsonFile.variables };
        }

        let binFilePath = path.join(currentBinPath, binaryFile);

        var options = { cwd: currentBinPath, env : env, detached: detachedProcess, stdio: stdioopt };

        apiProcess = cProcess.spawn(binFilePath, parameters, options);

        if (!detachedProcess) {
            apiProcess.stdout.on('data', (data) => {
                console.log(`stdout: ${data.toString()}`);
            });

            apiProcess.stderr.on('data', (data) => {
                console.log(`stderr: ${data.toString()}`);
            });
        }

        apiProcess.on('close', (code) => {
            console.log(`.NET process exited with code ${code}`);
            if (!ignoreApiProcessClosed) {
                if (code != 0) {
                    console.log(`Will quit Electron, as exit code != 0 (got ${code})`);
                }
                app.exit(code);
            }
            else if (os.platform() === 'darwin') {
                //There is a bug on the updater on macOS never quiting and starting the update process
                //We give Squirrel.Mac enough time to access the update file, and then just force-exit here
                setTimeout(() => app.exit(0), 30_000);
            }
        });


        apiProcess.stdin.setEncoding = 'utf-8';
        apiProcess.stdin.write('Auth=' + auth + '\n');
        apiProcess.stdin.end();

        if (detachedProcess) {
            console.log('Detached from .NET process');
            apiProcess.unref();
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
        console.log('.NET watch Port: ' + aspCoreBackendPort);
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

        let env = process.env;

        if (manifestJsonFile.hasOwnProperty('variables')) {
            env = { ...env, ...manifestJsonFile.variables };
        }

        var options = { cwd: currentBinPath, env: env, detached: detachedProcess, stdio: stdioopt };

        apiProcess = cProcess.spawn('dotnet', parameters, options);

        if (!detachedProcess) {
            apiProcess.stdout.on('data', (data) => {
                console.log(`stdout: ${data.toString()}`);
            });

            apiProcess.stderr.on('data', (data) => {
                console.log(`stderr: ${data.toString()}`);
            });
        }

        apiProcess.on('close', (code) => {
            console.log(`.NET process exited with code ${code}`);
            if (!ignoreApiProcessClosed) {
                if (code != 0) {
                    console.log(`Will quit Electron, as exit code != 0 (got ${code})`);
                }
                console.log('Will quit Electron now');

                app.exit(code);
            }
            else if (os.platform() === 'darwin') {
                console.log('.NET process and Electron has a pending update, will force quit in 10s...');
                //There is a bug on the updater on macOS never quiting and starting the update process
                //We give Squirrel.Mac enough time to access the update file, and then just force-exit here
                setTimeout(() => app.exit(0), 10_000);
            }
            else {
                console.log('.NET process and Electron has a pending update...');
            }
        });

        if (detachedProcess) {
            console.log('Detached from ASP.NET process');
            apiProcess.unref();
        }
    }
}

function getEnvironmentParameter() {
    if (manifestJsonFile.environment) {
        return '--environment=' + manifestJsonFile.environment;
    }

    return '';
}


//This code is derived from gh/sindresorhus/shell-path/, gh/sindresorhus/shell-env/, gh/sindresorhus/default-shell/, gh/chalk/ansi-regex, all under MIT license
function fixPath() {
    if (process.platform === 'win32') {
        return;
    }
    const shellFromEnv = shellEnvSync();

    if (process.env.PATH) {
        console.log("Started with PATH = " + process.env.PATH);
    }

    if (shellFromEnv) {
        process.env.PATH = shellFromEnv;
    } else {
        process.env.PATH = [
            './node_modules/.bin',
            '/.nodebrew/current/bin',
            '/usr/local/bin',
            process.env.PATH,
        ].join(':'); //macOS and Linux path separator is ':'
    }
    console.log("Using PATH = " + process.env.PATH);
}

function shellEnvSync() {
    const args = [
        '-ilc',
        'echo -n "_SHELL_ENV_DELIMITER_"; env; echo -n "_SHELL_ENV_DELIMITER_"; exit',
    ];

    const env = {
        // Disables Zsh auto-update that can block the process.
        DISABLE_AUTO_UPDATE: 'true',
    };

    let shell = process.env.SHELL || '/bin/sh';

    if (process.platform === 'darwin') {
        shell = process.env.SHELL || '/bin/zsh';
    }

    try {
        let { stdout } = cProcess.spawnSync(shell, args, { env });
        if (Buffer.isBuffer(stdout)) {
            stdout = stdout.toString();
        }
        return parseEnv(stdout);
    } catch (error) {
        if (shell) {
            throw error;
        } else {
            return process.env;
        }
    }
}

function parseEnv(envString) {
    const returnValue = {};

    if (envString) {
        envString = envString.split('_SHELL_ENV_DELIMITER_')[1];
        for (const line of envString.replace(ansiRegex(), '').split('\n').filter(line => Boolean(line))) {
            const [key, ...values] = line.split('=');
            returnValue[key.toUpperCase()] = values.join('=');
        }
    }

    return returnValue["PATH"];


    function ansiRegex() {
        const pattern = [
            '[\\u001B\\u009B][[\\]()#;?]*(?:(?:(?:(?:;[-a-zA-Z\\d\\/#&.:=?%@~_]+)*|[a-zA-Z\\d]+(?:;[-a-zA-Z\\d\\/#&.:=?%@~_]*)*)?\\u0007)',
            '(?:(?:\\d{1,4}(?:;\\d{0,4})*)?[\\dA-PR-TZcf-ntqry=><~]))'
        ].join('|');

        return new RegExp(pattern, 'g');
    }
}
