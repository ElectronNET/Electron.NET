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
        safeConsole.log(`[SignalRBridge] Connecting to ${this.hubUrl}`);

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(this.hubUrl)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.None) // Disable SignalR logging to avoid EPIPE
            .build();

        // Handle reconnection
        this.connection.onreconnecting((error) => {
            safeConsole.log(`[SignalRBridge] Connection lost. Reconnecting...`, error);
            this.isConnected = false;
        });

        this.connection.onreconnected((connectionId) => {
            safeConsole.log(`[SignalRBridge] Reconnected with ID: ${connectionId}`);
            this.isConnected = true;
        });

        this.connection.onclose((error) => {
            safeConsole.log(`[SignalRBridge] Connection closed`, error);
            this.isConnected = false;
        });

        // Set up handlers for messages from .NET
        this.setupMessageHandlers();

        try {
            await this.connection.start();
            this.isConnected = true;
            safeConsole.log(`[SignalRBridge] Connected successfully`);
            
            // Register with the hub
            await this.connection.invoke('RegisterElectronClient');
            safeConsole.log(`[SignalRBridge] Registered as Electron client`);
            
            return true;
        } catch (err) {
            safeConsole.error(`[SignalRBridge] Connection failed:`, err);
            this.isConnected = false;
            return false;
        }
    }

    setupMessageHandlers() {
        // Handle generic events from .NET - this is where .NET's Emit() calls arrive
        this.connection.on('event', (eventName, ...args) => {
            safeConsole.log(`[SignalRBridge] Received event: ${eventName}`);
            
            // Check if we have handlers registered for this event
            if (this.eventHandlers.has(eventName)) {
                const handlers = this.eventHandlers.get(eventName);
                handlers.forEach(handler => {
                    try {
                        handler(...args);
                    } catch (err) {
                        safeConsole.error(`[SignalRBridge] Error in event handler for ${eventName}:`, err);
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
        safeConsole.log(`[SignalRBridge] Registered handler for event: ${eventName}`);
    }

    // Socket.io compatibility: emit event (send to .NET)
    async emit(eventName, ...args) {
        if (!this.isConnected) {
            safeConsole.log(`[SignalRBridge] Cannot emit ${eventName} - not connected`);
            return;
        }

        try {
            safeConsole.log(`[SignalRBridge] Emitting event: ${eventName}`);
            await this.connection.invoke('ElectronEvent', eventName, ...args);
        } catch (err) {
            safeConsole.error(`[SignalRBridge] Error emitting ${eventName}:`, err);
            throw err;
        }
    }

    async disconnect() {
        if (this.connection) {
            await this.connection.stop();
            this.isConnected = false;
            safeConsole.log(`[SignalRBridge] Disconnected`);
        }
    }
}

module.exports = { SignalRBridge };
