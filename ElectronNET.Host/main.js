// const { app, BrowserWindow } = require('electron');
const io = require('socket.io')(3000);

// let window;

// app.on('ready', () => {



// });

io.on('connection', (socket) => {
    console.log('a user connected');

    socket.on('createBrowserWindow', (options) => {
        console.log(options);
    });
});

