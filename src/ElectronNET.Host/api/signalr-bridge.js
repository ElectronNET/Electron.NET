// SignalR connection module for Electron.NET
const signalR = require('@microsoft/signalr');

// Safe console wrapper that catches EPIPE errors
const safeConsole = {
    log: (...args) => {
        try {
            console.log(...args);
        } catch (e) {
            // Ignore EPIPE errors when console is detached
        }
    },
    error: (...args) => {
        try {
            console.error(...args);
        } catch (e) {
            // Ignore EPIPE errors when console is detached
        }
    }
};

class SignalRBridge {
    constructor(hubUrl) {
        this.hubUrl = hubUrl;
        this.connection = null;
        this.isConnected = false;
        this.eventHandlers = new Map(); // For socket.io-style .on() handlers
        this.callIdCounter = 0;
    }

    async connect() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(this.hubUrl)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        // Handle reconnection
        this.connection.onreconnecting((error) => {
            console.error(`[SignalRBridge] Connection lost. Reconnecting...`, error);
            this.isConnected = false;
        });

        this.connection.onreconnected((connectionId) => {
            this.isConnected = true;
        });

        this.connection.onclose((error) => {
            if (error) {
                console.error(`[SignalRBridge] Connection closed:`, error);
            }
            this.isConnected = false;
        });

        // Set up handlers for messages from .NET
        this.setupMessageHandlers();

        try {
            await this.connection.start();
            this.isConnected = true;
            
            // Register with the hub
            await this.connection.invoke('RegisterElectronClient');
            
            return true;
        } catch (err) {
            console.error(`[SignalRBridge] Connection failed:`, err);
            this.isConnected = false;
            return false;
        }
    }

    setupMessageHandlers() {
        // Handle generic events from .NET - this is where .NET's Emit() calls arrive
        this.connection.on('event', (eventName, args) => {
            // args is an array from .NET - spread it when calling handlers
            const argsArray = Array.isArray(args) ? args : [args];
            
            // Check if we have handlers registered for this event
            if (this.eventHandlers.has(eventName)) {
                const handlers = this.eventHandlers.get(eventName);
                handlers.forEach(handler => {
                    try {
                        handler(...argsArray);
                    } catch (err) {
                        console.error(`[SignalRBridge] Error in event handler for ${eventName}:`, err);
                    }
                });
            }
        });
    }

    // Socket.io compatibility: register event handler
    on(eventName, callback) {
        if (!this.eventHandlers.has(eventName)) {
            this.eventHandlers.set(eventName, []);
        }
        this.eventHandlers.get(eventName).push(callback);
    }

    // Socket.io compatibility: emit event (send to .NET)
    async emit(eventName, ...args) {
        if (!this.isConnected) {
            console.warn(`[SignalRBridge] Cannot emit ${eventName} - not connected`);
            return;
        }

        try {
            // Always pass args as an array to match C# method signature
            await this.connection.invoke('ElectronEvent', eventName, args);
        } catch (err) {
            console.error(`[SignalRBridge] Error emitting ${eventName}:`, err);
            throw err;
        }
    }

    async disconnect() {
        if (this.connection) {
            await this.connection.stop();
            this.isConnected = false;
        }
    }
}

module.exports = { SignalRBridge };
