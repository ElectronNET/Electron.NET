const { app, BrowserWindow } = require('electron');
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
        console.log(`stdout: ${data}`);
    });
});

io.on('connection', (socket) => {
    console.log('a user connected');

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
});

