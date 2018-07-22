const { app } = require('electron');
// yf add
const { BrowserWindow, dialog, shell } = require('electron')

const fs = require('fs');
const path = require('path');
const process = require('child_process').spawn;
const portfinder = require('detect-port');
let io, browserWindows, ipc, apiProcess, loadURL;
let appApi, menu, dialogApi, notification, tray, webContents;
let globalShortcut, shellApi, screen, clipboard;

// yf add
let loadingWindow;
let mainWindowId;
let countDownInterval;

// yf add
const manifestJsonFile = require("./bin/electron.manifest.json");
if (manifestJsonFile.singleInstance) {
    const shouldQuit = app.makeSingleInstance((commandLine, workingDirectory) => {
        mainWindowId && BrowserWindow.fromId(mainWindowId) && BrowserWindow.fromId(mainWindowId).show();
    });
    if (shouldQuit) {
        app.quit();
        return;
    }
}

app.on('ready', () => {

    // yf add 
    startLoadingWindow();

    portfinder(8000, (error, port) => {
        startSocketApiBridge(port);
    });
});

function startSocketApiBridge(port) {
    io = require('socket.io')(port);

    // yf add
    startAspCoreBackend(port);

    io.on('connection', (socket) => {

        global.elesocket = socket;
        global.elesocket.setMaxListeners(0);
        console.log('ASP.NET Core Application connected...', 'global.elesocket', global.elesocket.id, new Date());

        appApi = require('./api/app')(socket, app);
        browserWindows = require('./api/browserWindows')(socket);
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
    });
}

function startAspCoreBackend(electronPort) {
    portfinder(8000, (error, electronWebPort) => {
        loadURL = `http://localhost:${electronWebPort}`
        const parameters = [`/electronPort=${electronPort}`, `/electronWebPort=${electronWebPort}`];

        const manifestFile = require("./bin/electron.manifest.json");
        let binaryFile = manifestFile.executable;

        const os = require("os");
        if (os.platform() === "win32") {
            binaryFile = binaryFile + '.exe';
        }

        const binFilePath = path.join(__dirname, 'bin', binaryFile);
        var options = { cwd: path.join(__dirname, 'bin') };
        apiProcess = process(binFilePath, parameters, options);

        apiProcess.stdout.on('data', (data) => {
            var text = data.toString();
            console.log(`stdout: ${data.toString()}`);

            // yf add
            if (text.indexOf(manifestFile.mainWindowShowed) > -1 &&
                loadingWindow && !loadingWindow.isDestroyed()) {
                loadingWindow.close();

                mainWindowId = parseInt(text.replace(`${manifestFile.mainWindowShowed}:`, "").trim());
            }
        });
    });
}

// yf add
function startLoadingWindow() {
    let loadingUrl = manifestJsonFile.loadingUrl;
    let icon = manifestJsonFile.icon;
    if (loadingUrl) {
        loadingWindow = new BrowserWindow({
            width: manifestJsonFile.width,
            height: manifestJsonFile.height,
            transparent: true,
            frame: false,
            show: false,
            devTools: true,
            icon: path.join(__dirname, icon)
        })
        if (manifestJsonFile.devTools) {
            loadingWindow.webContents.openDevTools();
        }
        loadingWindow.loadURL(loadingUrl);
        loadingWindow.once('ready-to-show', () => {
            loadingWindow.show()

            // 激活倒计时
            activeCountDowInterval(manifestJsonFile)
        })
        loadingWindow.on('closed', () => {
            loadingWindow = null

            clearInterval(countDownInterval)
        })
    }
}

function activeCountDowInterval(manifestJsonFile) {
    if (!manifestJsonFile.timeout || !manifestJsonFile.timeout.limit)
        return

    let limitSecond = manifestJsonFile.timeout.limit
    let currentSecond = 0;
    countDownInterval = setInterval(() => {
        currentSecond++;
        if (currentSecond < limitSecond)
            return;

        clearInterval(countDownInterval);

        dialog.showMessageBox(loadingWindow, {
            type: manifestJsonFile.timeout.messageBox.type || 'error',
            buttons: manifestJsonFile.timeout.messageBox.buttons || ["前往安装"],
            title: manifestJsonFile.timeout.messageBox.title || '文件缺失提示',
            message: manifestJsonFile.timeout.messageBox.message || '计算机缺少组件无法启动该程序，点击前往安装组件后重试',
        }, (res, isChecked) => {
            if (manifestJsonFile.timeout.help)
                shell.openExternal(manifestJsonFile.timeout.help)
            app.quit();
        });

    }, 1000)
}

//app.on('activate', () => {
    // On macOS it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
//    if (win === null) {
//        createWindow();
//    }
//});