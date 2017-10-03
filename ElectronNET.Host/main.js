const { app, BrowserWindow, Notification } = require('electron');
const io = require('socket.io')(3000);
const path = require('path');

let window;
let apiProcess;

app.on('ready', () => {
    const process = require('child_process').spawn;

    //  run server
    var apipath = path.join(__dirname, '..\\ElectronNET.WebApp\\bin\\dist\\win\\ElectronNET.WebApp.exe');
    apiProcess = process(apipath);

    apiProcess.stdout.on('data', (data) => {
        var text = data.toString();
        console.log(`stdout: ${data.toString()}`);
    });
});

io.on('connection', (socket) => {
    console.log('ASP.NET Core Application connected...');

    socket.on('createBrowserWindow', (options) => {
        console.log(options);
        options.show = true;

        window = new BrowserWindow(options);
        window.loadURL('http://localhost:5000');

        window.on('closed', function () {
            mainWindow = null;
            apiProcess = null;
        });
    });

    socket.on('createNotification', (options) => {
        const notification = new Notification(options);
        notification.show();
    });
});

